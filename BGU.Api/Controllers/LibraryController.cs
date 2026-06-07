using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.Library.Requests;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize]
public class LibraryController(ILibraryService libraryService) : ControllerBase
{
    [HttpGet(ApiEndPoints.LibraryBooks.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? query,
        [FromQuery] string? category,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var res = await libraryService.GetAllAsync(query, category, status, page, pageSize);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [HttpGet(ApiEndPoints.LibraryBooks.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var res = await libraryService.GetByIdAsync(id);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean,Teacher")]
    [HttpPost(ApiEndPoints.LibraryBooks.Create)]
    public async Task<IActionResult> Create([FromForm] UpsertLibraryBookRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var res = await libraryService.CreateAsync(request, userId);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean,Teacher")]
    [HttpPut(ApiEndPoints.LibraryBooks.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromForm] UpsertLibraryBookRequest request)
    {
        var res = await libraryService.UpdateAsync(id, request);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [Authorize(Roles = "Dean,Teacher")]
    [HttpDelete(ApiEndPoints.LibraryBooks.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await libraryService.DeleteAsync(id);
        Response.StatusCode = res.StatusCode;
        return new ObjectResult(res);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndPoints.LibraryBooks.Cover)]
    public async Task<IActionResult> Cover([FromRoute] string id)
    {
        var res = await libraryService.GetCoverAsync(id);
        if (!res.IsSucceeded || res.Data is null)
        {
            Response.StatusCode = res.StatusCode;
            return new ObjectResult(res);
        }

        return PhysicalFile(
            res.Data.FullPath,
            res.Data.ContentType,
            enableRangeProcessing: true);
    }

    [HttpGet(ApiEndPoints.LibraryBooks.Read)]
    public async Task<IActionResult> Read([FromRoute] string id)
    {
        var res = await libraryService.GetReadFileAsync(id);
        if (!res.IsSucceeded || res.Data is null)
        {
            Response.StatusCode = res.StatusCode;
            return new ObjectResult(res);
        }

        return PhysicalFile(
            res.Data.FullPath,
            res.Data.ContentType,
            enableRangeProcessing: true);
    }
    
}
