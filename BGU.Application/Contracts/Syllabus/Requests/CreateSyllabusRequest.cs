using Microsoft.AspNetCore.Http;

namespace BGU.Application.Contracts.Syllabus.Requests;

public sealed record CreateSyllabusRequest(IFormFile File, string TaughtSubjectId, string WwwRootPath);