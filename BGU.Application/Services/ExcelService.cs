using System.Drawing;
using System.Globalization;
using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BGU.Application.Services;

public class ExcelService : IExcelService {
    public async Task<List<StudentDto>> ParseStudentExcelAsync(Stream fileStream) {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var students = new List<StudentDto>();

        using (var package = new ExcelPackage(fileStream)) {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) {
                try {
                    var student = new StudentDto(
                        Name: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Surname: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        MiddleName: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        UserName: worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                        GroupName: worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                        AdmissionScore: double.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0")
                    );

                    students.Add(student);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error parsing row {row}: {ex.Message}");
                }
            }
        }

        return students;
    }


    public async Task<List<TeacherDto>> ParseTeacherExcelAsync(Stream fileStream) {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var teachers = new List<TeacherDto>();

        using (var package = new ExcelPackage(fileStream)) {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++) {
                try {
                    teachers.Add(new TeacherDto(
                        Name: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Surname: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        MiddleName: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        UserName: worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                        DepartmentName: worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                        Position: ParseEnum<TeachingPosition>(worksheet.Cells[row, 6].Value)
                    ));
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error parsing Teacher row {row}: {ex.Message}");
                }
            }
        }

        return teachers;
    }

    private T ParseEnum<T>(object cellValue) where T : struct, Enum {
        if (cellValue == null)
            throw new ArgumentException("Cell value is null");

        var stringValue = cellValue.ToString()?.Trim();

        if (Enum.TryParse(stringValue, ignoreCase: true, out T result) &&
            Enum.IsDefined(typeof(T), result))
            return result;

        throw new ArgumentException($"Cannot parse '{stringValue}' to {typeof(T).Name}");
    }

    public byte[] GenerateUserResultsExcel(List<BulkImportResult> results) {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Import Results");

        // Split + order (successful with non-empty temp password first)
        var successful = results
            .Where(r => r.Success)
            .OrderByDescending(r => !string.IsNullOrWhiteSpace(r.TemporaryPassword))
            .ThenBy(r => r.FullName)
            .ToList();

        var failed = results
            .Where(r => !r.Success)
            .ToList();

        // Layout
        const int titleRow = 1;
        const int headerRow = 2;
        const int dataStartRow = 3;

        const int successColStart = 1; // A
        const int successColCount = 3; // Name, FIN, TempPwd

        const int gapCols = 2; // пустой промежуток между таблицами
        const int failColStart = successColStart + successColCount + gapCols; // F
        const int failColCount = 3; // Name, FIN, Message
        var summaryColLabel = failColStart + failColCount + 2; // after FAILED block => I
        var summaryColValue = summaryColLabel + 1; // J

        ws.Cells[1, summaryColLabel].Value = "Import Summary";
        ws.Cells[1, summaryColLabel].Style.Font.Bold = true;
        ws.Cells[1, summaryColLabel].Style.Font.Size = 14;

// optional: merge title across 2 cols (I:J)
        ws.Cells[1, summaryColLabel, 1, summaryColValue].Merge = true;

        ws.Cells[2, summaryColLabel].Value = "Total Processed:";
        ws.Cells[2, summaryColValue].Value = results.Count;

        var successCount = results.Count(r => r.Success);
        var failCount = results.Count - successCount;

        ws.Cells[3, summaryColLabel].Value = "Successful:";
        ws.Cells[3, summaryColValue].Value = successCount;
        ws.Cells[3, summaryColValue].Style.Font.Color.SetColor(Color.Green);

        ws.Cells[4, summaryColLabel].Value = "Failed:";
        ws.Cells[4, summaryColValue].Value = failCount;
        ws.Cells[4, summaryColValue].Style.Font.Color.SetColor(Color.Red);
        // Section titles (merged)
        ws.Cells[titleRow, successColStart, titleRow, successColStart + successColCount - 1].Merge = true;
        ws.Cells[titleRow, successColStart].Value = "SUCCESSFUL";
        ws.Cells[titleRow, successColStart].Style.Font.Bold = true;

        ws.Cells[titleRow, failColStart, titleRow, failColStart + failColCount - 1].Merge = true;
        ws.Cells[titleRow, failColStart].Value = "FAILED";
        ws.Cells[titleRow, failColStart].Style.Font.Bold = true;

        // Headers - Successful
        ws.Cells[headerRow, successColStart + 0].Value = "Name Surname";
        ws.Cells[headerRow, successColStart + 1].Value = "FIN Code";
        ws.Cells[headerRow, successColStart + 2].Value = "Temporary Password";

        // Headers - Failed
        ws.Cells[headerRow, failColStart + 0].Value = "Name Surname";
        ws.Cells[headerRow, failColStart + 1].Value = "FIN Code";
        ws.Cells[headerRow, failColStart + 2].Value = "Message";

        // Header style (одинаковый для обоих блоков)
        void StyleHeaderRange(int row, int colStart, int colCount) {
            using var range = ws.Cells[row, colStart, row, colStart + colCount - 1];
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        StyleHeaderRange(headerRow, successColStart, successColCount);
        StyleHeaderRange(headerRow, failColStart, failColCount);

        // Data - Successful
        var r1 = dataStartRow;
        foreach (var r in successful) {
            ws.Cells[r1, successColStart + 0].Value = r.FullName ?? "";
            ws.Cells[r1, successColStart + 1].Value = r.UserName ?? "";
            ws.Cells[r1, successColStart + 2].Value = r.TemporaryPassword ?? "";
            r1++;
        }

        // Data - Failed
        var r2 = dataStartRow;
        foreach (var r in failed) {
            ws.Cells[r2, failColStart + 0].Value = r.FullName ?? "";
            ws.Cells[r2, failColStart + 1].Value = r.UserName ?? "";
            ws.Cells[r2, failColStart + 2].Value = r.Message ?? "";
            r2++;
        }

        // Nice-to-have: freeze pane at first data row (optional)
        ws.View.FreezePanes(dataStartRow, 1);

        // Autofit only used area
        if (ws.Dimension != null)
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }
}