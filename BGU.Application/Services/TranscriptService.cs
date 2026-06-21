using System.Globalization;
using System.Reflection;
using BGU.Application.Common;
using BGU.Application.Dtos.Transcript;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BGU.Application.Services;

public class TranscriptService(AppDbContext context) : ITranscriptService
{
    private const string BorderColor = "#111111";
    private const float BorderWidth = 0.45f;

    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
    private static readonly SvgImage? AzerbaijanEmblem = LoadAzerbaijanEmblem();

    private async Task<TranscriptQueryResult?> GetDataAsync(string userId)
    {
        var student = await context.Students
            .AsNoTracking()
            .Where(s => s.AppUserId == userId)
            .Select(s => new
            {
                s.Id,
                s.Gpa,
                StudentNumber = s.AppUser.UserName,
                FullName = (s.AppUser.Surname + " " + s.AppUser.Name + " " + s.AppUser.MiddleName).Trim(),
                Faculty = s.Faculty.Name,
                Specialization = s.Specialization.Name,
                Group = s.Group.Code,
                EducationLanguage = s.Group.EducationLanguage.ToString(),
                EducationLevel = s.Group.EducationLevel.ToString(),
                AdmissionYear = s.AdmissionYear.FirstYear
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
            return null;
        }

        var results = await context.StudentSubjectResults
            .AsNoTracking()
            .Where(r => r.StudentId == student.Id && r.IsFinalized)
            .OrderBy(r => r.TaughtSubject.Semester)
            .ThenBy(r => r.TaughtSubject.Subject.Name)
            .Select(r => new
            {
                SubjectName = r.TaughtSubject.Subject.Name,
                SubjectCode = r.TaughtSubject.Code,
                Credits = r.TaughtSubject.Subject.CreditsNumber,
                Semester = r.TaughtSubject.Semester,
                r.FinalGrade
            })
            .ToListAsync();

        var rows = results
            .Select(r => new TranscriptRow(
                r.SubjectName,
                r.SubjectCode,
                r.Credits,
                r.Semester,
                r.FinalGrade,
                ToLetter(r.FinalGrade)))
            .ToList();

        return new TranscriptQueryResult(
            student.FullName,
            student.StudentNumber ?? student.Id,
            student.Faculty,
            student.Specialization,
            student.Group,
            ToEducationLanguage(student.EducationLanguage),
            ToEducationLevel(student.EducationLevel),
            student.AdmissionYear,
            student.Gpa,
            rows);
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
                page.MarginHorizontal(8, Unit.Millimetre);
                page.MarginVertical(8, Unit.Millimetre);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(7.4f));

                page.Content().Column(col =>
                {
                    col.Spacing(3);

                    col.Item().Element(ComposeHeader);
                    col.Item().Element(container => ComposeStudentInfo(container, data));
                    col.Item().PaddingTop(1).AlignCenter().Text("AKADEMİK TRANSKRİPT")
                        .Bold().FontSize(12);
                    col.Item().Element(container => ComposeYearsGrid(container, data));
                    col.Item().Height(3, Unit.Millimetre);
                    col.Item().Element(ComposeSignatures);
                });
            });
        }).GeneratePdf();

        return ApiResult<byte[]>.Success(bytes);
    }

    private static void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(2);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().Text("Ünvan: Möhsün Sənani 14").FontSize(7.5f);
                table.Cell().AlignRight().Text("Tel: 012 5946986").FontSize(7.5f);
            });

            if (AzerbaijanEmblem is not null)
            {
                col.Item()
                    .AlignCenter()
                    .Width(16, Unit.Millimetre)
                    .Height(18, Unit.Millimetre)
                    .Svg(AzerbaijanEmblem)
                    .FitArea();
            }

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Cell().AlignCenter().Text("Azərbaycan Respublikası Təhsil Nazirliyi").FontSize(8.2f);
                table.Cell().AlignCenter().Text("The Ministry of Education of Azerbaijan Republic").FontSize(8.2f);
                table.Cell().AlignCenter().Text("BAKI QIZLAR UNİVERSİTETİ").Bold().FontSize(9.2f);
                table.Cell().AlignCenter().Text("Baku Girls University").Bold().FontSize(9.2f);
            });
        });
    }

    private static void ComposeStudentInfo(IContainer container, TranscriptQueryResult data)
    {
        container.Column(col =>
        {
            col.Spacing(2);

            void InfoLine(string left, string right)
            {
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Text(left).FontSize(7.6f);
                    table.Cell().Text(right).FontSize(7.6f);
                });
            }

            InfoLine($"Fakültə: {data.Faculty}", $"İxtisas: {data.Specialization}");
            InfoLine($"Tələbə № {data.StudentNumber}", "İxtisaslaşma:");
            InfoLine($"Soyadı və adı: {data.FullName}", $"Təhsil pilləsi: {data.EducationLevel}");
            InfoLine("Doğum tarixi:", $"Tədris dili: {data.EducationLanguage}");
        });
    }

    private static void ComposeYearsGrid(IContainer container, TranscriptQueryResult data)
    {
        container.Column(column =>
        {
            column.Spacing(3);
            column.Item().Row(row => ComposeYearGridRow(row, data, 1, 2));
            column.Item().Row(row => ComposeYearGridRow(row, data, 3, 4));
        });
    }

    private static void ComposeYearGridRow(RowDescriptor row, TranscriptQueryResult data, int leftYear, int rightYear)
    {
        row.RelativeItem().Element(container => ComposeYearTable(container, data, leftYear));
        row.ConstantItem(3, Unit.Millimetre);
        row.RelativeItem().Element(container => ComposeYearTable(container, data, rightYear));
    }

    private static void ComposeYearTable(IContainer container, TranscriptQueryResult data, int year)
    {
        var rows = data.Rows
            .Where(row => ToAcademicYear(row.Semester) == year)
            .OrderBy(row => row.Semester)
            .ThenBy(row => row.SubjectName)
            .ToList();

        var yearCredits = rows.Sum(row => row.Credits);
        var yearAverage = CalculateWeightedAverage(rows);

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(12, Unit.Millimetre);
                columns.RelativeColumn();
                columns.ConstantColumn(15, Unit.Millimetre);
                columns.ConstantColumn(19, Unit.Millimetre);
            });

            TitleCell(table.Cell().ColumnSpan(4))
                .AlignCenter()
                .Text($"{year}-ci tədris ili")
                .Bold();

            table.Header(header =>
            {
                HeaderCell(header.Cell()).AlignCenter().Text("Semestr").Bold();
                HeaderCell(header.Cell()).AlignCenter().Text("Fənnin adı").Bold();
                HeaderCell(header.Cell()).AlignCenter().Text("Kreditin miqdarı").Bold();
                HeaderCell(header.Cell()).AlignCenter().Text("Topladığı bal").Bold();
            });

            if (rows.Count == 0)
            {
                BodyCell(table.Cell()).AlignCenter().Text("1-2");
                BodyCell(table.Cell()).Text("Fənn yoxdur");
                BodyCell(table.Cell()).AlignCenter().Text("0");
                BodyCell(table.Cell()).AlignCenter().Text("-");
            }
            else
            {
                foreach (var semester in new[] { (year * 2) - 1, year * 2 })
                {
                    var semesterRows = rows
                        .Where(row => row.Semester == semester)
                        .OrderBy(row => row.SubjectName)
                        .ToList();

                    if (semesterRows.Count == 0)
                    {
                        continue;
                    }

                    BodyCell(table.Cell().RowSpan((uint)semesterRows.Count))
                        .AlignMiddle()
                        .AlignCenter()
                        .Text(ToSemesterWithinYear(semester).ToString(InvariantCulture))
                        .Bold();

                    foreach (var row in semesterRows)
                    {
                        BodyCell(table.Cell()).Text(row.SubjectName);
                        BodyCell(table.Cell()).AlignCenter().Text(row.Credits.ToString(InvariantCulture));
                        BodyCell(table.Cell()).AlignCenter().Text($"{FormatScore(row.FinalGrade)} {row.Letter}");
                    }
                }
            }

            SummaryCell(table.Cell().ColumnSpan(4))
                .AlignCenter()
                .Column(summary =>
                {
                    summary.Spacing(1);
                    summary.Item().Text($"Kredit: {yearCredits}    ÜOMG: {FormatAverage(yearAverage)}").Bold();

                    if (year == 4)
                    {
                        var totalCredits = data.Rows.Sum(row => row.Credits);
                        var totalAverage = CalculateWeightedAverage(data.Rows);
                        summary.Item().Text(
                                $"Cəmi kredit sayı: {totalCredits}    Cəmi ÜOMG: {FormatAverage(totalAverage)}")
                            .Bold();
                    }
                });
        });
    }

    private static IContainer TitleCell(IContainer container)
    {
        return container
            .Border(BorderWidth)
            .BorderColor(BorderColor)
            .Background(Colors.Grey.Lighten5)
            .MinHeight(10)
            .PaddingHorizontal(2)
            .PaddingVertical(1)
            .DefaultTextStyle(style => style.FontSize(7).Bold());
    }

    private static void ComposeSignatures(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(11);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(45, Unit.Millimetre);
                });

                table.Cell().Text("Rektor:").FontSize(9);
                table.Cell().Text("S.A.Rəhimova").FontSize(9);
            });

            col.Item().Text($"Verilmə tarixi {DateTime.Now:dd.MM.yyyy}").FontSize(9);
        });
    }

    private static IContainer HeaderCell(IContainer container)
    {
        return container
            .Border(BorderWidth)
            .BorderColor(BorderColor)
            .Background(Colors.Grey.Lighten4)
            .MinHeight(10)
            .PaddingHorizontal(2)
            .PaddingVertical(1)
            .DefaultTextStyle(style => style.FontSize(6.2f).Bold());
    }

    private static IContainer BodyCell(IContainer container)
    {
        return container
            .Border(BorderWidth)
            .BorderColor(BorderColor)
            .MinHeight(8)
            .PaddingHorizontal(2)
            .PaddingVertical(1)
            .DefaultTextStyle(style => style.FontSize(6.1f));
    }

    private static IContainer SummaryCell(IContainer container)
    {
        return container
            .Border(BorderWidth)
            .BorderColor(BorderColor)
            .MinHeight(10)
            .PaddingHorizontal(2)
            .PaddingVertical(1)
            .DefaultTextStyle(style => style.FontSize(6.2f));
    }

    private static double CalculateWeightedAverage(IEnumerable<TranscriptRow> rows)
    {
        var rowList = rows.ToList();
        var credits = rowList.Sum(row => row.Credits);
        return credits == 0 ? 0 : rowList.Sum(row => row.FinalGrade * row.Credits) / credits;
    }

    private static string ToEducationLanguage(string value)
    {
        return value switch
        {
            "Azerbaijani" => "Azərbaycan",
            "Russian" => "Rus",
            "English" => "İngilis",
            _ => value
        };
    }

    private static string ToEducationLevel(string value)
    {
        return value switch
        {
            "Bachelor" => "bakalavr",
            "Master" => "magistr",
            _ => value
        };
    }

    private static string FormatScore(double value)
    {
        return Math.Abs(value - Math.Round(value)) < 0.001
            ? Math.Round(value).ToString("0", InvariantCulture)
            : value.ToString("0.##", InvariantCulture).Replace('.', ',');
    }

    private static string FormatAverage(double value)
    {
        return value.ToString("0.00", InvariantCulture).Replace('.', ',');
    }

    private static string ToLetter(double grade)
    {
        return grade switch
        {
            >= 91 => "A",
            >= 81 => "B",
            >= 71 => "C",
            >= 61 => "D",
            >= 51 => "E",
            _ => "F"
        };
    }

    private static int ToSemesterWithinYear(int semester)
    {
        return semester % 2 == 0 ? 2 : 1;
    }

    private static int ToAcademicYear(int semester) => (int)Math.Ceiling(semester / 2.0);

    private static SvgImage? LoadAzerbaijanEmblem()
    {
        var assembly = typeof(TranscriptService).Assembly;
        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("Assets.azerbaijan-emblem.svg", StringComparison.Ordinal));

        if (resourceName is null)
        {
            return null;
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        return SvgImage.FromText(reader.ReadToEnd());
    }
}
