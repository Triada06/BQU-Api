using BGU.Api.Helpers;
using BGU.Application.Contracts.Seminars.Requests;
using BGU.Application.Dtos.Seminar;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Teacher")]
public class SeminarController(ISeminarService seminarService, ISeminarRepository seminarRepository) : ControllerBase
{
    [HttpGet(ApiEndPoints.Seminar.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var seminars = (await seminarRepository.GetAllAsync(page, pageSize, false,
            x =>
                x.Include(s => s.Student)
                    .ThenInclude(s => s.AppUser)
                    .Include(s => s.TaughtSubject))).ToList();

        if (seminars.Count == 0)
        {
            return NoContent();
        }

        var returnDtos = seminars.Select(s =>
            new GetSeminar(s.Id, s.GotAt, s.Grade, s.Student.AppUser.Name, s.StudentId, s.TaughtSubjectId));

        return new ObjectResult(returnDtos);
    }
    
    [HttpPost(ApiEndPoints.Seminar.Create)]
    public async Task<IActionResult> Create([FromRoute] string studentId, [FromRoute] string taughtSubjectId)
    {
        var res = await seminarService.CreateAsync(new CreateSeminarRequest(studentId, taughtSubjectId));
        return new ObjectResult(res);
    }
    [HttpDelete(ApiEndPoints.Seminar.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await seminarService.DeleteAsync(id);
        return new ObjectResult(res);
    }
    
}