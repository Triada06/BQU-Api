using BGU.Application.Contracts.Colloquium.Requests;
using BGU.Application.Contracts.Colloquium.Responses;
using BGU.Application.Dtos.Colloquiums;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class ColloquiumService(
    ITaughtSubjectRepository taughtSubjectRepository,
    IStudentRepository studentRepository,
    IColloquiumRepository colloquiumRepository): IColloquiumService {
    public async Task<CreateColloquiumResponse> CreateAsync(CreateColloquiumRequest request) {
        if (!await taughtSubjectRepository.AnyAsync(x => x.Id == request.TaughtSubjectId)) {
            return new CreateColloquiumResponse(null, StatusCode.BadRequest, false,
                $"Subject with an Id of {request.TaughtSubjectId} does not exist");
        }

        if (!await studentRepository.AnyAsync(x => x.Id == request.StudentId)) {
            return new CreateColloquiumResponse(null, StatusCode.BadRequest, false,
                $"Student with an Id of {request.StudentId} does not exist");
        }

        var colloquium = new Colloquiums {
            Grade = request.Grade ?? Grade.None,
            Date = request.Date,
            IsConfirmed = false,
            StudentId = request.StudentId,
            TaughtSubjectId = request.TaughtSubjectId,
        };
        return await colloquiumRepository.CreateAsync(colloquium)
            ? new CreateColloquiumResponse(colloquium.Id, StatusCode.Created, true,
                ResponseMessages.CreatedSuccessfully)
            : new CreateColloquiumResponse(null, StatusCode.BadRequest, false,
                "An error occured while creating colloquium");
    }

    public async Task<DeleteColloquiumResponse> DeleteAsync(string id) {
        var coll = await colloquiumRepository.GetByIdAsync(id, tracking: true);
        if (coll == null) {
            return new DeleteColloquiumResponse(StatusCode.BadRequest, false,
                "Not Found");
        }

        return await colloquiumRepository.DeleteAsync(coll)
            ? new DeleteColloquiumResponse(StatusCode.Ok, true,
                ResponseMessages.Success)
            : new DeleteColloquiumResponse(StatusCode.BadRequest, false, "An error occured while deleting colloquium");
    }

    public async Task<GetAllColloquiumResponse> GetAllAsync(string taughtSubjectId) {
        var colls = await colloquiumRepository.FindAsync(x => x.TaughtSubjectId == taughtSubjectId,
            include: x => x
                .Include(c => c.Student)
                .ThenInclude(st => st.AppUser)
                .Include(c => c.TaughtSubject), tracking: false);

        if (colls.Count == 0 || colls.Any(x => x is null)) {
            return new GetAllColloquiumResponse([], StatusCode.Ok, true, ResponseMessages.Success);
        }

        return new GetAllColloquiumResponse(
            colls.Select(x =>
                new ColloquiumDto(x!.Id, x.TaughtSubject.Code, x.Student.AppUser.Name + x.Student.AppUser.Surname,
                    x.Grade, x.Date)),
            StatusCode.Ok, true,
            ResponseMessages.Success);
    }
}