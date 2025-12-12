using BGU.Application.Contracts.Syllabus.Requests;
using BGU.Application.Contracts.Syllabus.Responses;
using BGU.Application.Dtos.Syllabus;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class SyllabusService(ISyllabusRepository syllabusRepository, ITaughtSubjectRepository taughtSubjectRepository)
    : ISyllabusService
{
    public async Task<CreateSyllabusResponse> CreateAsync(CreateSyllabusRequest request)
    {
        if (await taughtSubjectRepository.AnyAsync(ts => ts.Syllabus != null))
        {
            return new CreateSyllabusResponse(null, StatusCode.Conflict, false,
                "This subjet already has a syllabus");
        }

        var fileName = Guid.NewGuid() + "-" + request.File.Name;
        var filePath = Path.Combine(request.WwwRootPath, fileName);

        await using (var writer = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(writer);
        }

        var syllabus = new Syllabus
        {
            Name = fileName,
            FilePath = filePath,
            TaughtSubjectId = request.TaughtSubjectId,
        };

        if (!await syllabusRepository.CreateAsync(syllabus))
        {
            return new CreateSyllabusResponse(null, StatusCode.InternalServerError, false,
                ResponseMessages.Failed);
        }

        return new CreateSyllabusResponse(syllabus.Id, StatusCode.Ok, true,
            ResponseMessages.CreatedSuccessfully);
    }

    public async Task<UpdateSyllabusResponse> UpdateAsync(UpdateSyllabusRequest request)
    {
        var syllabus = await syllabusRepository.GetByIdAsync(request.Id, tracking: true);
        if (syllabus is null)
        {
            return new UpdateSyllabusResponse(null, StatusCode.BadRequest, false, ResponseMessages.BadRequest);
        }

        if (File.Exists(syllabus.FilePath))
        {
            File.Delete(syllabus.FilePath);
        }

        var newFileName = Guid.NewGuid() + "-" + request.File.Name;
        var newFilePath = Path.Combine(request.WwwRootPath, newFileName);

        await using (var writer = new FileStream(newFilePath, FileMode.Create))
        {
            await request.File.CopyToAsync(writer);
        }

        syllabus.Name = newFileName;
        syllabus.FilePath = newFilePath;

        if (!await syllabusRepository.UpdateAsync(syllabus))
        {
            return new UpdateSyllabusResponse(null, StatusCode.InternalServerError, false, ResponseMessages.Failed);
        }

        return new UpdateSyllabusResponse(syllabus.Id, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<DeleteSyllabusResponse> DeleteAsync(string id)
    {
        var syllabus = await syllabusRepository.GetByIdAsync(id, tracking: true);
        if (syllabus is null)
        {
            return new DeleteSyllabusResponse(StatusCode.BadRequest, false, ResponseMessages.BadRequest);
        }

        if (!await syllabusRepository.DeleteAsync(syllabus))
        {
            return new DeleteSyllabusResponse(StatusCode.InternalServerError, false, ResponseMessages.Failed);
        }

        if (File.Exists(syllabus.FilePath))
        {
            File.Delete(syllabus.FilePath);
        }

        return new DeleteSyllabusResponse(StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetByIdSyllabusResponse> GetByIdAsync(string id)
    {
        var syllabus = await syllabusRepository.GetByIdAsync(id, tracking: true);
        if (syllabus is null)
        {
            return new GetByIdSyllabusResponse(null, StatusCode.BadRequest, false, ResponseMessages.BadRequest);
        }

        var bytes = await File.ReadAllBytesAsync(syllabus.FilePath);
        return new GetByIdSyllabusResponse(new GetSyllabusDto(bytes, syllabus.Name), StatusCode.Ok, true,
            ResponseMessages.Success);
    }
}