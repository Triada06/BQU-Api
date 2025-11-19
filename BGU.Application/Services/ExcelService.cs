using BGU.Application.Dtos.AdmissionYear;
using BGU.Application.Dtos.ClassTime;
using BGU.Application.Dtos.Department;
using BGU.Application.Dtos.Faculty;
using BGU.Application.Dtos.Group;
using BGU.Application.Dtos.LectureHall;
using BGU.Application.Dtos.Specialization;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Subject;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using OfficeOpenXml;

namespace BGU.Application.Services;

public class ExcelService : IExcelService
{
    public async Task<List<AdmissionYearDto>> ParseAdmissionYearExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<AdmissionYearDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new AdmissionYearDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        FirstYear: int.Parse(worksheet.Cells[row, 2].Value?.ToString()),
                        SecondYear: int.Parse(worksheet.Cells[row, 3].Value?.ToString()),
                        Operation: worksheet.Cells[row, 4].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing AdmissionYear row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateAdmissionYearTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("AdmissionYears");

            // Headers
            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "FirstYear";
            worksheet.Cells[1, 3].Value = "SecondYear";
            worksheet.Cells[1, 4].Value = "Operation (CREATE/UPDATE/DELETE)";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Example rows
            worksheet.Cells[2, 1].Value = "";
            worksheet.Cells[2, 2].Value = 2024;
            worksheet.Cells[2, 3].Value = 2025;
            worksheet.Cells[2, 4].Value = "CREATE";

            // Dropdown for Operation
            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 4, 1000, 4].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<FacultyDto>> ParseFacultyExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<FacultyDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new FacultyDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Name: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        Operation: worksheet.Cells[row, 3].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Faculty row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateFacultyTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Faculties");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 3])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 1].Value = "";
            worksheet.Cells[2, 2].Value = "Faculty of Computer Science";
            worksheet.Cells[2, 3].Value = "CREATE";

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 3, 1000, 3].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<DepartmentDto>> ParseDepartmentExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<DepartmentDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new DepartmentDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Name: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        FacultyId: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        Operation: worksheet.Cells[row, 4].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Department row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateDepartmentTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Departments");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "FacultyId";
            worksheet.Cells[1, 4].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "Department of Software Engineering";
            worksheet.Cells[2, 3].Value = "faculty-id-here";
            worksheet.Cells[2, 4].Value = "CREATE";

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 4, 1000, 4].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<SpecializationDto>> ParseSpecializationExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<SpecializationDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new SpecializationDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Name: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        FacultyId: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        Operation: worksheet.Cells[row, 4].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Specialization row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateSpecializationTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Specializations");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "FacultyId";
            worksheet.Cells[1, 4].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "Computer Science";
            worksheet.Cells[2, 3].Value = "faculty-id-here";
            worksheet.Cells[2, 4].Value = "CREATE";

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 4, 1000, 4].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }


    public async Task<List<GroupDto>> ParseGroupExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<GroupDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new GroupDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Code: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        AdmissionYearId: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        EducationLanguage: ParseEnum<EducationLanguage>(worksheet.Cells[row, 4].Value),
                        EducationLevel: ParseEnum<EducationLevel>(worksheet.Cells[row, 5].Value),
                        SpecializationId: worksheet.Cells[row, 6].Value?.ToString()?.Trim(),
                        Operation: worksheet.Cells[row, 7].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Group row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateGroupTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Groups");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Code";
            worksheet.Cells[1, 3].Value = "AdmissionYearId";
            worksheet.Cells[1, 4].Value = "EducationLanguage (Azerbaijani/Russian/English)";
            worksheet.Cells[1, 5].Value = "EducationLevel (Бакалавриат/Магистратура/Докторантура)";
            worksheet.Cells[1, 6].Value = "SpecializationId";
            worksheet.Cells[1, 7].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "CS-2024-1";
            worksheet.Cells[2, 3].Value = "admission-year-id";
            worksheet.Cells[2, 4].Value = "English";
            worksheet.Cells[2, 5].Value = "Бакалавриат";
            worksheet.Cells[2, 6].Value = "spec-id";
            worksheet.Cells[2, 7].Value = "CREATE";

            var langValidation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 4, 1000, 4].Address);
            langValidation.Formula.Values.Add("Azerbaijani");
            langValidation.Formula.Values.Add("Russian");
            langValidation.Formula.Values.Add("English");

            var levelValidation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 5, 1000, 5].Address);
            levelValidation.Formula.Values.Add("Бакалавриат");
            levelValidation.Formula.Values.Add("Магистратура");
            levelValidation.Formula.Values.Add("Докторантура");

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 7, 1000, 7].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<SubjectDto>> ParseSubjectExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<SubjectDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new SubjectDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Name: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        CreditsNumber: int.Parse(worksheet.Cells[row, 3].Value?.ToString()),
                        TeacherCode: worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                        DepartmentId: worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                        Operation: worksheet.Cells[row, 6].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Subject row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateSubjectTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Subjects");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "CreditsNumber";
            worksheet.Cells[1, 4].Value = "TeacherCode";
            worksheet.Cells[1, 5].Value = "DepartmentId";
            worksheet.Cells[1, 6].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "Data Structures";
            worksheet.Cells[2, 3].Value = 6;
            worksheet.Cells[2, 4].Value = "CS101";
            worksheet.Cells[2, 5].Value = "dept-id";
            worksheet.Cells[2, 6].Value = "CREATE";

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 6, 1000, 6].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<LectureHallDto>> ParseLectureHallExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<LectureHallDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new LectureHallDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Name: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        Capacity: int.Parse(worksheet.Cells[row, 3].Value?.ToString()),
                        Operation: worksheet.Cells[row, 4].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing LectureHall row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateLectureHallTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("LectureHalls");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Capacity";
            worksheet.Cells[1, 4].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 4])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "Hall A-101";
            worksheet.Cells[2, 3].Value = 50;
            worksheet.Cells[2, 4].Value = "CREATE";

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 4, 1000, 4].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<ClassTimeDto>> ParseClassTimeExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<ClassTimeDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new ClassTimeDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Start: TimeSpan.Parse(worksheet.Cells[row, 2].Value?.ToString()),
                        End: TimeSpan.Parse(worksheet.Cells[row, 3].Value?.ToString()),
                        DaysOfTheWeek: ParseEnum<DaysOfTheWeek>(worksheet.Cells[row, 4].Value),
                        Operation: worksheet.Cells[row, 5].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing ClassTime row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateClassTimeTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("ClassTimes");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Start (HH:MM:SS)";
            worksheet.Cells[1, 3].Value = "End (HH:MM:SS)";
            worksheet.Cells[1, 4].Value = "DayOfWeek (Monday/Tuesday/Wednesday/Thursday/Friday)";
            worksheet.Cells[1, 5].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "09:00:00";
            worksheet.Cells[2, 3].Value = "10:30:00";
            worksheet.Cells[2, 4].Value = "Monday";
            worksheet.Cells[2, 5].Value = "CREATE";

            var dayValidation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 4, 1000, 4].Address);
            dayValidation.Formula.Values.Add("Monday");
            dayValidation.Formula.Values.Add("Tuesday");
            dayValidation.Formula.Values.Add("Wednesday");
            dayValidation.Formula.Values.Add("Thursday");
            dayValidation.Formula.Values.Add("Friday");

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 5, 1000, 5].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<TaughtSubjectDto>> ParseTaughtSubjectExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<TaughtSubjectDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new TaughtSubjectDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        SubjectId: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        TeacherId: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        GroupId: worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                        Operation: worksheet.Cells[row, 5].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing TaughtSubject row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateTaughtSubjectTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("TaughtSubjects");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "SubjectId";
            worksheet.Cells[1, 3].Value = "TeacherId";
            worksheet.Cells[1, 4].Value = "GroupId";
            worksheet.Cells[1, 5].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "subject-id-here";
            worksheet.Cells[2, 3].Value = "teacher-id-here";
            worksheet.Cells[2, 4].Value = "group-id-here";
            worksheet.Cells[2, 5].Value = "CREATE";

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 5, 1000, 5].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    public async Task<List<TeacherDto>> ParseTeacherExcelAsync(Stream fileStream)
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        var items = new List<TeacherDto>();

        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    items.Add(new TeacherDto(
                        Id: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                        Email: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                        Name: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                        Surname: worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                        MiddleName: worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                        PinCode: worksheet.Cells[row, 6].Value?.ToString()?.Trim(),
                        Gender: worksheet.Cells[row, 6].Value?.ToString()?.Trim().ToUpper().FirstOrDefault() ?? 'U',
                        BornDate: DateTime.Parse(worksheet.Cells[row, 8].Value?.ToString()),
                        DepartmentId: worksheet.Cells[row, 9].Value?.ToString()?.Trim(),
                        Position: ParseEnum<TeachingPosition>(worksheet.Cells[row, 10].Value),
                        ContractType: ParseEnum<TypeOfContract>(worksheet.Cells[row, 11].Value),
                        State: ParseEnum<State>(worksheet.Cells[row, 12].Value),
                        Operation: worksheet.Cells[row, 14].Value?.ToString()?.Trim().ToUpper() ?? "CREATE"
                    ));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing Teacher row {row}: {ex.Message}");
                }
            }
        }

        return items;
    }

    public async Task<byte[]> GenerateTeacherTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Teachers");

            worksheet.Cells[1, 1].Value = "Id (Leave empty for CREATE)";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Name";
            worksheet.Cells[1, 4].Value = "Surname";
            worksheet.Cells[1, 5].Value = "MiddleName";
            worksheet.Cells[1, 6].Value = "PinCode";
            worksheet.Cells[1, 7].Value = "Gender (M/F)";
            worksheet.Cells[1, 8].Value = "BornDate (YYYY-MM-DD)";
            worksheet.Cells[1, 9].Value = "DepartmentId";
            worksheet.Cells[1, 10].Value = "Position";
            
            worksheet.Cells[1, 11].Value = "ContractType";
            worksheet.Cells[1, 12].Value = "State";
            worksheet.Cells[1, 14].Value = "Operation (CREATE/UPDATE/DELETE)";

            using (var range = worksheet.Cells[1, 1, 1, 14])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            worksheet.Cells[2, 2].Value = "teacher@example.com";
            worksheet.Cells[2, 3].Value = "John";
            worksheet.Cells[2, 4].Value = "Smith";
            worksheet.Cells[2, 5].Value = "Michael";
            worksheet.Cells[2, 6].Value = "1234567";
            worksheet.Cells[2, 7].Value = "M";
            worksheet.Cells[2, 8].Value = "1980-05-15";
            worksheet.Cells[2, 9].Value = "dept-id-here";
            worksheet.Cells[2, 10].Value = "Профессор";
            worksheet.Cells[2, 11].Value = "Постоянная";
            worksheet.Cells[2, 12].Value = "Полный";
            worksheet.Cells[2, 13].Value = 5000;
            worksheet.Cells[2, 14].Value = "CREATE";

            var positionValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 10, 1000, 10].Address);
            positionValidation.Formula.Values.Add("СтаршийУчитель");
            positionValidation.Formula.Values.Add("Учитель");
            positionValidation.Formula.Values.Add("СтаршийЛаборант");
            positionValidation.Formula.Values.Add("Лаборант");
            positionValidation.Formula.Values.Add("ЗавКафедры");
            positionValidation.Formula.Values.Add("ЗавЛаборатории");
            positionValidation.Formula.Values.Add("Доцент");
            positionValidation.Formula.Values.Add("Профессор");
            positionValidation.Formula.Values.Add("Тютор");
            positionValidation.Formula.Values.Add("Клерк");

            var contractValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 11, 1000, 11].Address);
            contractValidation.Formula.Values.Add("Постоянная");
            contractValidation.Formula.Values.Add("Временно");
            contractValidation.Formula.Values.Add("Извне");

            var stateValidation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 12, 1000, 12].Address);
            stateValidation.Formula.Values.Add("Полставки");
            stateValidation.Formula.Values.Add("Полный");
            stateValidation.Formula.Values.Add("Почасово");

            var operationValidation =
                worksheet.DataValidations.AddListValidation(worksheet.Cells[2, 14, 1000, 14].Address);
            operationValidation.Formula.Values.Add("CREATE");
            operationValidation.Formula.Values.Add("UPDATE");
            operationValidation.Formula.Values.Add("DELETE");

            worksheet.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }
    }

   public async Task<List<CreateStudentDto>> ParseStudentExcelAsync(Stream fileStream)
{
    ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

    var students = new List<CreateStudentDto>();

    using (var package = new ExcelPackage(fileStream))
    {
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension.Rows;

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                // Parse enums from string
                var educationLangStr = worksheet.Cells[row, 12].Value?.ToString()?.Trim();
                var formOfEducationStr = worksheet.Cells[row, 13].Value?.ToString()?.Trim();

                // Parse BornDate correctly
                var bornCellValue = worksheet.Cells[row, 7].Value;
                DateTime bornDate;

                if (bornCellValue is double numericDate) 
                {
                    // Excel stores dates as numbers sometimes
                    bornDate = DateTime.FromOADate(numericDate);
                }
                else
                {
                    // fallback to string parsing
                    bornDate = DateTime.Parse(bornCellValue?.ToString() 
                                ?? throw new Exception($"Invalid BornDate at row {row}"));
                }

                var student = new CreateStudentDto(
                    Email: worksheet.Cells[row, 1].Value?.ToString()?.Trim(),
                    Name: worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                    Surname: worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                    MiddleName: worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                    PinCode: worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                    Gender: worksheet.Cells[row, 6].Value?.ToString()?.Trim().ToUpper().FirstOrDefault() ?? 'U',
                    BornDate: bornDate,
                    FacultyId: worksheet.Cells[row, 8].Value?.ToString()?.Trim(),
                    SpecializationId: worksheet.Cells[row, 9].Value?.ToString()?.Trim(),
                    GroupId: worksheet.Cells[row, 10].Value?.ToString()?.Trim(),
                    AdmissionYearId: worksheet.Cells[row, 11].Value?.ToString()?.Trim(),
                    EducationLanguage: Enum.Parse<EducationLanguage>(educationLangStr, ignoreCase: true),
                    FormOfEducation: Enum.Parse<FormOfEducation>(formOfEducationStr, ignoreCase: true),
                    DecreeNumber: int.Parse(worksheet.Cells[row, 14].Value?.ToString() ?? "0"),
                    AdmissionScore: double.Parse(worksheet.Cells[row, 15].Value?.ToString() ?? "0")
                );

                students.Add(student);
            }
            catch (Exception ex)
            {
                // Log the row number + error, continue parsing next rows
                Console.WriteLine($"Error parsing row {row}: {ex.Message}");
            }
        }
    }

    return students;
}

    public async Task<byte[]> GenerateStudentTemplateAsync()
    {
        ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Students");

            // Headers
            worksheet.Cells[1, 1].Value = "Email";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Surname";
            worksheet.Cells[1, 4].Value = "MiddleName";
            worksheet.Cells[1, 5].Value = "PinCode";
            worksheet.Cells[1, 6].Value = "Gender (M/F)";
            worksheet.Cells[1, 7].Value = "BornDate (YYYY-MM-DD)";
            worksheet.Cells[1, 8].Value = "FacultyId";
            worksheet.Cells[1, 9].Value = "SpecializationId";
            worksheet.Cells[1, 10].Value = "GroupId";
            worksheet.Cells[1, 11].Value = "AdmissionYearId";
            worksheet.Cells[1, 12].Value = "EducationLanguage";
            worksheet.Cells[1, 13].Value = "FormOfEducation";
            worksheet.Cells[1, 14].Value = "DecreeNumber";
            worksheet.Cells[1, 15].Value = "AdmissionScore";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 15])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Example row
            worksheet.Cells[2, 1].Value = "student@example.com";
            worksheet.Cells[2, 2].Value = "John";
            worksheet.Cells[2, 3].Value = "Doe";
            worksheet.Cells[2, 4].Value = "Smith";
            worksheet.Cells[2, 5].Value = "1234567";
            worksheet.Cells[2, 6].Value = "M";
            worksheet.Cells[2, 7].Value = "2000-01-15";
            worksheet.Cells[2, 8].Value = "faculty-id-123";
            worksheet.Cells[2, 9].Value = "spec-id-456";
            worksheet.Cells[2, 10].Value = "group-id-789";
            worksheet.Cells[2, 11].Value = "year-id-2024";
            worksheet.Cells[2, 12].Value = "English";
            worksheet.Cells[2, 13].Value = "InPerson";
            worksheet.Cells[2, 14].Value = "12345";
            worksheet.Cells[2, 15].Value = "650.5";

            worksheet.Cells.AutoFitColumns();

            return await Task.FromResult(package.GetAsByteArray());
        }
    }

    private T ParseEnum<T>(object cellValue) where T : struct, Enum
    {
        if (cellValue == null)
            throw new ArgumentException("Cell value is null");

        var stringValue = cellValue.ToString()?.Trim();

        if (int.TryParse(stringValue, out int numValue))
        {
            if (Enum.IsDefined(typeof(T), numValue))
                return (T)(object)numValue;
        }

        if (Enum.TryParse<T>(stringValue, ignoreCase: true, out T result))
            return result;

        throw new ArgumentException($"Cannot parse '{stringValue}' to {typeof(T).Name}");
    }
}