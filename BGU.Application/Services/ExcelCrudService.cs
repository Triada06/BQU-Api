using BGU.Application.Common;
using BGU.Application.Dtos.AdmissionYear;
using BGU.Application.Dtos.ClassTime;
using BGU.Application.Dtos.Department;
using BGU.Application.Dtos.Faculty;
using BGU.Application.Dtos.Group;
using BGU.Application.Dtos.LectureHall;
using BGU.Application.Dtos.Specialization;
using BGU.Application.Dtos.Subject;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class ExcelCrudService(AppDbContext dbContext, UserManager<AppUser> userManager) : IExcelCrudService
{
    public async Task<List<BulkImportResult>> ProcessAdmissionYearsAsync(List<AdmissionYearDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newYear = new AdmissionYear
                        {
                            FirstYear = item.FirstYear,
                            SecondYear = item.SecondYear
                        };
                        dbContext.AdmissionYears.Add(newYear);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = $"{item.FirstYear}-{item.SecondYear}",
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newYear.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = $"{item.FirstYear}-{item.SecondYear}",
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingYear = await dbContext.AdmissionYears.FindAsync(item.Id);
                        if (existingYear == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = $"{item.FirstYear}-{item.SecondYear}",
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingYear.FirstYear = item.FirstYear;
                        existingYear.SecondYear = item.SecondYear;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = $"{item.FirstYear}-{item.SecondYear}",
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingYear.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = $"{item.FirstYear}-{item.SecondYear}",
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var yearToDelete = await dbContext.AdmissionYears.FindAsync(item.Id);
                        if (yearToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = $"{item.FirstYear}-{item.SecondYear}",
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.AdmissionYears.Remove(yearToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = $"{item.FirstYear}-{item.SecondYear}",
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = $"{item.FirstYear}-{item.SecondYear}",
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessFacultiesAsync(List<FacultyDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newFaculty = new Faculty { Name = item.Name };
                        dbContext.Faculties.Add(newFaculty);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newFaculty.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingFaculty = await dbContext.Faculties.FindAsync(item.Id);
                        if (existingFaculty == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingFaculty.Name = item.Name;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingFaculty.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var facultyToDelete = await dbContext.Faculties.FindAsync(item.Id);
                        if (facultyToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.Faculties.Remove(facultyToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Name,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessDepartmentsAsync(List<DepartmentDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newDepartment = new Department { Name = item.Name };
                        dbContext.Departments.Add(newDepartment);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newDepartment.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingDepartment = await dbContext.Departments.FindAsync(item.Id);
                        if (existingDepartment == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingDepartment.Name = item.Name;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingDepartment.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var departmentToDelete = await dbContext.Departments.FindAsync(item.Id);
                        if (departmentToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.Departments.Remove(departmentToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Name,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessSpecializationsAsync(List<SpecializationDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newSpecialization = new Specialization { Name = item.Name };
                        dbContext.Specializations.Add(newSpecialization);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newSpecialization.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingSpecialization = await dbContext.Specializations.FindAsync(item.Id);
                        if (existingSpecialization == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingSpecialization.Name = item.Name;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingSpecialization.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var specializationToDelete = await dbContext.Specializations.FindAsync(item.Id);
                        if (specializationToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.Specializations.Remove(specializationToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Name,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessGroupsAsync(List<GroupDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newGroup = new Group { Code = item.Code };
                        dbContext.Groups.Add(newGroup);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Code,
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newGroup.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Code,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingGroup = await dbContext.Groups.FindAsync(item.Id);
                        if (existingGroup == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Code,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingGroup.Code = item.Code;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Code,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingGroup.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Code,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var groupToDelete = await dbContext.Groups.FindAsync(item.Id);
                        if (groupToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Code,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.Groups.Remove(groupToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Code,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Code,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessSubjectsAsync(List<SubjectDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newSubject = new Subject { Name = item.Name };
                        dbContext.Subjects.Add(newSubject);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newSubject.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingSubjects = await dbContext.Subjects.FindAsync(item.Id);
                        if (existingSubjects == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingSubjects.Name = item.Name;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingSubjects.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var subjectToDelete = await dbContext.Subjects.FindAsync(item.Id);
                        if (subjectToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.Subjects.Remove(subjectToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Name,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessLectureHallsAsync(List<LectureHallDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newHall = new LectureHall { Name = item.Name };
                        dbContext.LectureHalls.Add(newHall);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newHall.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingHall = await dbContext.LectureHalls.FindAsync(item.Id);
                        if (existingHall == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingHall.Name = item.Name;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingHall.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var hallToDelete = await dbContext.LectureHalls.FindAsync(item.Id);
                        if (hallToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Name,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.LectureHalls.Remove(hallToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Name,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Name,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessClassTimesAsync(List<ClassTimeDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newClassTime = new ClassTime();
                        dbContext.ClassTimes.Add(newClassTime);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newClassTime.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingClassTime = await dbContext.ClassTimes.FindAsync(item.Id);
                        if (existingClassTime == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingClassTime.Start = item.Start;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = item.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var classTimeToDelete = await dbContext.ClassTimes.FindAsync(item.Id);
                        if (classTimeToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.ClassTimes.Remove(classTimeToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessTaughtSubjectsAsync(List<TaughtSubjectDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        var newTaughtSubject = new TaughtSubject();
                        dbContext.TaughtSubjects.Add(newTaughtSubject);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Operation = "CREATE",
                            Success = true,
                            Message = "Created successfully",
                            EntityId = newTaughtSubject.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingTaughtObject = await dbContext.TaughtSubjects.FindAsync(item.Id);
                        if (existingTaughtObject == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        existingTaughtObject.GroupId = item.GroupId;
                        existingTaughtObject.SubjectId = item.SubjectId;
                        existingTaughtObject.TeacherId = item.TeacherId;
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = item.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var taughtSubjectToDelete = await dbContext.TaughtSubjects.FindAsync(item.Id);
                        if (taughtSubjectToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Operation = "DELETE",
                                Success = false,
                                Message = "Entity not found"
                            });
                            continue;
                        }

                        dbContext.TaughtSubjects.Remove(taughtSubjectToDelete);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    public async Task<List<BulkImportResult>> ProcessTeachersAsync(List<TeacherDto> items)
    {
        var results = new List<BulkImportResult>();

        foreach (var item in items)
        {
            try
            {
                switch (item.Operation)
                {
                    case "CREATE":
                        // Check if email already exists
                        var existingUser = await userManager.FindByEmailAsync(item.Email);
                        if (existingUser != null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "CREATE",
                                Success = false,
                                Message = "Email already exists"
                            });
                            continue;
                        }

                        // Validate department exists
                        var departmentExists = await dbContext.Departments.AnyAsync(d => d.Id == item.DepartmentId);
                        if (!departmentExists)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "CREATE",
                                Success = false,
                                Message = "Department not found"
                            });
                            continue;
                        }

                        // Create AppUser
                        var user = new AppUser
                        {
                            UserName = item.Email.Split('@')[0],
                            Email = item.Email,
                            Name = item.Name,
                            Surname = item.Surname,
                            MiddleName = item.MiddleName,
                            Pin = item.PinCode,
                            Gender = item.Gender,
                            BornDate = item.BornDate
                        };

                        var tempPassword = GenerateTemporaryPassword();
                        var createResult = await userManager.CreateAsync(user, tempPassword);

                        if (!createResult.Succeeded)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "CREATE",
                                Success = false,
                                Message = string.Join(", ", createResult.Errors.Select(e => e.Description))
                            });
                            continue;
                        }

                        await userManager.AddToRoleAsync(user, "Teacher");

                        // Create Teacher entity
                        var teacher = new Teacher
                        {
                            AppUserId = user.Id,
                            TeacherAcademicInfo = new TeacherAcademicInfo
                            {
                                DepartmentId = item.DepartmentId,
                                TeachingPosition = item.Position,
                                TypeOfContract = item.ContractType,
                                State = item.State,
                            }
                        };

                        dbContext.Teachers.Add(teacher);
                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Email,
                            Operation = "CREATE",
                            Success = true,
                            Message = $"Created successfully. Temporary password: {tempPassword}",
                            EntityId = teacher.Id
                        });
                        break;

                    case "UPDATE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Id is required for update"
                            });
                            continue;
                        }

                        var existingTeacher = await dbContext.Teachers
                            .Include(t => t.AppUser)
                            .Include(t => t.TeacherAcademicInfo)
                            .FirstOrDefaultAsync(t => t.Id == item.Id);

                        if (existingTeacher == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Teacher not found"
                            });
                            continue;
                        }

                        // Validate department exists
                        var deptExists = await dbContext.Departments.AnyAsync(d => d.Id == item.DepartmentId);
                        if (!deptExists)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "UPDATE",
                                Success = false,
                                Message = "Department not found"
                            });
                            continue;
                        }

                        // Update AppUser info
                        existingTeacher.AppUser.Name = item.Name;
                        existingTeacher.AppUser.Surname = item.Surname;
                        existingTeacher.AppUser.MiddleName = item.MiddleName;
                        existingTeacher.AppUser.Pin = item.PinCode;
                        existingTeacher.AppUser.Gender = item.Gender;
                        existingTeacher.AppUser.BornDate = item.BornDate;

                        // Update email if changed
                        if (existingTeacher.AppUser.Email != item.Email)
                        {
                            var emailToken =
                                await userManager.GenerateChangeEmailTokenAsync(existingTeacher.AppUser, item.Email);
                            var emailResult =
                                await userManager.ChangeEmailAsync(existingTeacher.AppUser, item.Email, emailToken);

                            if (!emailResult.Succeeded)
                            {
                                results.Add(new BulkImportResult
                                {
                                    Identifier = item.Email,
                                    Operation = "UPDATE",
                                    Success = false,
                                    Message =
                                        $"Failed to update email: {string.Join(", ", emailResult.Errors.Select(e => e.Description))}"
                                });
                                continue;
                            }
                        }

                        await userManager.UpdateAsync(existingTeacher.AppUser);

                        // Update TeacherAcademicInfo
                        if (existingTeacher.TeacherAcademicInfo != null)
                        {
                            existingTeacher.TeacherAcademicInfo.DepartmentId = item.DepartmentId;
                            existingTeacher.TeacherAcademicInfo.TeachingPosition = item.Position;
                            existingTeacher.TeacherAcademicInfo.TypeOfContract = item.ContractType;
                            existingTeacher.TeacherAcademicInfo.State = item.State;
                        }
                        else
                        {
                            // Create academic info if it doesn't exist
                            existingTeacher.TeacherAcademicInfo = new TeacherAcademicInfo
                            {
                                DepartmentId = item.DepartmentId,
                                TeachingPosition = item.Position,
                                TypeOfContract = item.ContractType,
                                State = item.State,
                            };
                        }

                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Email,
                            Operation = "UPDATE",
                            Success = true,
                            Message = "Updated successfully",
                            EntityId = existingTeacher.Id
                        });
                        break;

                    case "DELETE":
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Id is required for delete"
                            });
                            continue;
                        }

                        var teacherToDelete = await dbContext.Teachers
                            .Include(t => t.AppUser)
                            .Include(t => t.TeacherAcademicInfo)
                            .FirstOrDefaultAsync(t => t.Id == item.Id);

                        if (teacherToDelete == null)
                        {
                            results.Add(new BulkImportResult
                            {
                                Identifier = item.Email,
                                Operation = "DELETE",
                                Success = false,
                                Message = "Teacher not found"
                            });
                            continue;
                        }

                        // Delete in order: TeacherAcademicInfo → Teacher → AppUser
                        if (teacherToDelete.TeacherAcademicInfo != null)
                        {
                            dbContext.TeacherAcademicInfos.Remove(teacherToDelete.TeacherAcademicInfo);
                        }

                        dbContext.Teachers.Remove(teacherToDelete);
                        await userManager.DeleteAsync(teacherToDelete.AppUser);

                        await dbContext.SaveChangesAsync();

                        results.Add(new BulkImportResult
                        {
                            Identifier = item.Email,
                            Operation = "DELETE",
                            Success = true,
                            Message = "Deleted successfully",
                            EntityId = item.Id
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                results.Add(new BulkImportResult
                {
                    Identifier = item.Email,
                    Operation = item.Operation,
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private static string GenerateTemporaryPassword()
        => $"Temp{Guid.NewGuid().ToString().Substring(0, 8)}!";
}