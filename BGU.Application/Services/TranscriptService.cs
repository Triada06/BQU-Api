using BGU.Application.Common;
using BGU.Application.Dtos.Transcript;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BGU.Application.Services;

public class TranscriptService(AppDbContext context) : ITranscriptService
{
    private async Task<TranscriptQueryResult?> GetDataAsync(string userId)
    {
        var studentId = await context.Students.AsNoTracking().Where(s => s.AppUserId == userId).Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (studentId is null)
        {
            return null;
        }

        var student = await context.Students
            .AsNoTracking()
            .Where(s => s.Id ==studentId)
            .Select(s => new
            {
                s.Id,
                s.Gpa,
                FullName = $"{s.AppUser.Surname} {s.AppUser.Name} {s.AppUser.MiddleName}",
                // BirthDate = s.AppUser.BirthDate,
                Faculty = s.Faculty.Name,
                Specialization = s.Specialization.Name,
                Group = s.Group.Code,
                EducationLanguage = s.Group.EducationLanguage.ToString(),
                EducationLevel = s.Group.EducationLevel.ToString(),
                // AdmissionYear = s.AdmissionYear.Year,
                Results = context.StudentSubjectResults
                    .Where(r => r.StudentId == studentId && r.IsFinalized)
                    .Select(r => new
                    {
                        SubjectName = r.TaughtSubject.Subject.Name,
                        SubjectCode = r.TaughtSubject.Code,
                        Credits = r.TaughtSubject.Subject.CreditsNumber,
                        Semester = r.TaughtSubject.Semester, 
                        r.FinalGrade
                    })
                    .OrderBy(r => r.Semester)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return student is null
            ? null
            : new TranscriptQueryResult(student.FullName,
                // student.BirthDate,
                student.Faculty, student.Specialization, student.Group, student.EducationLanguage,
                student.EducationLevel,
                // student.AdmissionYear,
                student.Gpa, student.Results
                    .Select(r => new TranscriptRow(r.SubjectName, r.SubjectCode, r.Credits,
                        r.Semester, r.FinalGrade, ToLetter(r.FinalGrade)))
                    .ToList());
    }


    public async Task<ApiResult<byte[]>> GeneratePdfAsync(string studentId)
    {
        var data = await GetDataAsync(studentId);
        if (data is null)
        {
            return ApiResult<byte[]>.BadRequest($"Student {studentId} not found");
        }

        var bytes = Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(15, Unit.Millimetre);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9));

                page.Content().Column(col =>
                {
                    // header
                    col.Item().AlignCenter().Text("Bakı Qızlar Universiteti")
                        .Bold().FontSize(13);
                    col.Item().AlignCenter().Text("AKADEMİK TRANSKRİPT")
                        .Bold().FontSize(11);
                    col.Item().AlignCenter().Text("FORMA 3")
                        .Bold().FontSize(11);
                    col.Item().Height(6, Unit.Millimetre);

                    // student info grid
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        void InfoRow(string left, string right)
                        {
                            t.Cell().PaddingBottom(2).Text(left);
                            t.Cell().PaddingBottom(2).Text(right);
                        }

                        InfoRow($"Fakültə: {data.Faculty}", $"Qrup №: {data.Group}");
                        InfoRow($"Təhsil səviyyəsi: {data.EducationLevel}", $"Tədris dili: {data.EducationLanguage}");
                        InfoRow($"İxtisaslaşma: {data.Specialization}", "Təhsilalma forması: Əyani");
                        InfoRow($"Soyadı, adı və ata adı: {data.FullName}", ""); // todo: add birthday later on
                        // $"Doğum tarixi: {data.BirthDate:dd/MM/yyyy}");
                    });

                    col.Item().Height(5, Unit.Millimetre);

                    // group rows by academic year
                    var byYear = data.Rows
                        .GroupBy(r => ToAcademicYear(r.Semester))
                        .OrderBy(g => g.Key);

                    foreach (var yearGroup in byYear)
                    {
                        col.Item().Text($"Akademik il: {yearGroup.Key}").Bold();
                        col.Item().Height(2, Unit.Millimetre);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(20, Unit.Millimetre); // sem
                                c.RelativeColumn(); // name
                                c.ConstantColumn(28, Unit.Millimetre); // code
                                c.ConstantColumn(16, Unit.Millimetre); // credit
                                c.ConstantColumn(12, Unit.Millimetre); // grade
                                c.ConstantColumn(12, Unit.Millimetre); // letter
                            });

                            t.Header(h =>
                            {
                                void Th(string text) =>
                                    h.Cell().Background("#2c3e50").Padding(3)
                                        .Text(text).FontColor("#ffffff").Bold().FontSize(8);

                                Th("Sem");
                                Th("Fənnin adı");
                                Th("Kod");
                                Th("Kredit");
                                Th("Bal");
                                Th("Hərf");
                            });

                            var rowIndex = 0;
                            foreach (var row in yearGroup.OrderBy(r => r.Semester))
                            {
                                var bg = rowIndex++ % 2 == 0 ? "#ffffff" : "#f5f5f5";

                                void Td(string text, bool center = false)
                                {
                                    var cell = t.Cell().Background(bg).Border(0.3f)
                                        .BorderColor("#cccccc").Padding(3);
                                    var txt = cell.Text(text).FontSize(8);
                                    if (center) txt.AlignCenter();
                                }

                                var semCode = row.Semester % 2 == 1
                                    ? $"PY-{row.Semester}"
                                    : $"YZ-{row.Semester}";

                                Td(semCode, center: true);
                                Td(row.SubjectName);
                                Td(row.SubjectCode, center: true);
                                Td(row.Credits.ToString(), center: true);
                                Td(row.FinalGrade.ToString("F0"), center: true);
                                Td(row.Letter, center: true);
                            }
                        });

                        // year summary
                        var totalCredits = yearGroup.Sum(r => r.Credits);
                        var gpa = yearGroup.Sum(r => r.FinalGrade * r.Credits) / totalCredits;

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.ConstantColumn(40, Unit.Millimetre);
                            });
                            t.Cell().Border(0.3f).BorderColor("#cccccc").Padding(3)
                                .Text($"Kreditlərin sayı: {totalCredits}/{totalCredits}").Bold().FontSize(8);
                            t.Cell().Border(0.3f).BorderColor("#cccccc").Padding(3)
                                .AlignCenter().Text($"ÜOMG: {gpa:F4}").Bold().FontSize(8);
                        });

                        col.Item().Height(5, Unit.Millimetre);
                    }

                    // overall GPA
                    var totalAll = data.Rows.Sum(r => r.Credits);
                    var overallGpa = totalAll > 0
                        ? data.Rows.Sum(r => r.FinalGrade * r.Credits) / totalAll
                        : 0;

                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.ConstantColumn(40, Unit.Millimetre);
                        });
                        t.Cell().Border(0.3f).BorderColor("#cccccc").Padding(3)
                            .Text($"Ümumi kreditlər: {totalAll}/{totalAll}").Bold().FontSize(8);
                        t.Cell().Border(0.3f).BorderColor("#cccccc").Padding(3)
                            .AlignCenter().Text($"Ümumi ÜOMG: {overallGpa:F4}").Bold().FontSize(8);
                    });

                    col.Item().Height(10, Unit.Millimetre);

                    // signatures
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        void SigRow(string label)
                        {
                            t.Cell().PaddingBottom(8).Text(label).FontSize(8);
                            t.Cell().PaddingBottom(8).Text("_________________________").FontSize(8);
                        }

                        SigRow("Dekan");
                        SigRow("Dekan müavini");
                        SigRow("Tyutor");
                        SigRow("Verilmə tarixi");
                    });
                });
            });
        }).GeneratePdf();

        return ApiResult<byte[]>.Success(bytes);
    }

    public async Task<ApiResult<byte[]>> GenerateExcelAsync(string studentId)
    {
        var data = await GetDataAsync(studentId);
        if (data is null)
        {
            return ApiResult<byte[]>.BadRequest($"Student {studentId} not found");
        }

        ExcelPackage.License.SetNonCommercialOrganization("BQU LMS");

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Akademik Transkript");

        // header
        ws.Cells["A1"].Value = "Bakı Qızlar Universiteti - Akademik Transkript";
        ws.Cells["A1:F1"].Merge = true;
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Size = 14;
        ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // student info
        var infoStart = 3;

        void InfoRow(int row, string label, string value)
        {
            ws.Cells[row, 1].Value = label;
            ws.Cells[row, 1].Style.Font.Bold = true;
            ws.Cells[row, 2, row, 4].Merge = true;
            ws.Cells[row, 2].Value = value;
        }

        InfoRow(infoStart, "Ad, soyad:", data.FullName);
        // InfoRow(infoStart + 1, "Doğum tarixi:", data.BirthDate.ToString("dd/MM/yyyy"));
        InfoRow(infoStart + 2, "Fakültə:", data.Faculty);
        InfoRow(infoStart + 3, "İxtisaslaşma:", data.Specialization);
        InfoRow(infoStart + 4, "Qrup:", data.Group);
        InfoRow(infoStart + 5, "Ümumi GPA:", data.Gpa.ToString("F4"));

        var currentRow = infoStart + 7;

        var byYear = data.Rows
            .GroupBy(r => ToAcademicYear(r.Semester))
            .OrderBy(g => g.Key);

        var headerColor = System.Drawing.Color.FromArgb(44, 62, 80);
        var altColor = System.Drawing.Color.FromArgb(245, 245, 245);

        foreach (var yearGroup in byYear)
        {
            // year title
            ws.Cells[currentRow, 1, currentRow, 6].Merge = true;
            ws.Cells[currentRow, 1].Value = $"Akademik il: {yearGroup.Key}";
            ws.Cells[currentRow, 1].Style.Font.Bold = true;
            ws.Cells[currentRow, 1].Style.Font.Size = 11;
            ws.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(236, 240, 241));
            currentRow++;

            // table header
            string[] headers = ["Sem", "Fənnin adı", "Kod", "Kredit", "Bal", "Hərf"];
            for (var i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[currentRow, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(headerColor);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Gray);
            }

            currentRow++;

            var rowIndex = 0;
            foreach (var row in yearGroup.OrderBy(r => r.Semester))
            {
                var bg = rowIndex++ % 2 == 0 ? System.Drawing.Color.White : altColor;
                var semCode = row.Semester % 2 == 1 ? $"PY-{row.Semester}" : $"YZ-{row.Semester}";

                object[] values = [semCode, row.SubjectName, row.SubjectCode, row.Credits, row.FinalGrade, row.Letter];
                for (var i = 0; i < values.Length; i++)
                {
                    var cell = ws.Cells[currentRow, i + 1];
                    cell.Value = values[i];
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(bg);
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                    if (i != 1) cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                currentRow++;
            }

            // year summary
            var totalCredits = yearGroup.Sum(r => r.Credits);
            var gpa = yearGroup.Sum(r => r.FinalGrade * r.Credits) / totalCredits;

            ws.Cells[currentRow, 1, currentRow, 3].Merge = true;
            ws.Cells[currentRow, 1].Value = $"Kreditlərin sayı: {totalCredits}/{totalCredits}";
            ws.Cells[currentRow, 1].Style.Font.Bold = true;
            ws.Cells[currentRow, 4, currentRow, 6].Merge = true;
            ws.Cells[currentRow, 4].Value = $"ÜOMG: {gpa:F4}";
            ws.Cells[currentRow, 4].Style.Font.Bold = true;
            ws.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            currentRow += 2;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();
        ws.Column(2).Width = 45; // subject name col fixed width

        var bytes = await package.GetAsByteArrayAsync();

        return bytes is null
            ? ApiResult<byte[]>.SystemError("Failed to generate an excel sheet")
            : ApiResult<byte[]>.Success(bytes);
    }

    private static string ToLetter(double grade) => grade switch
    {
        >= 91 => "A",
        >= 81 => "B",
        >= 71 => "C",
        >= 61 => "D",
        >= 51 => "E",
        _ => "F"
    };

    private static int ToAcademicYear(int semester) => (int)Math.Ceiling(semester / 2.0);
}