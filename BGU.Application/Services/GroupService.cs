using System.Diagnostics.Contracts;
using BGU.Application.Contracts.Group;
using BGU.Application.Contracts.Group.Requests;
using BGU.Application.Contracts.Group.Responses;
using BGU.Application.Dtos.Group;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class GroupService(IGroupRepository groupRepository) : IGroupService
{
    public async Task<GetAllGroupsResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        var groups =
            (await groupRepository.GetAllAsync(page, pageSize, tracking,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
                        .Include(e => e.AdmissionYear)
            ))
            .Select(x => new GetGroupDto(x.Id, x.Code, x.Specialization.Name,
                DateTime.Now.Year - x.AdmissionYear.FirstYear, x.Students.Count));
        return new GetAllGroupsResponse(groups, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetByIdGroupsResponse> GetByIdAsync(string id, bool tracking = false)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
                        .Include(e => e.AdmissionYear), tracking
            );

        if (group is null)
        {
            return new GetByIdGroupsResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        return new GetByIdGroupsResponse(new GetGroupDto(group.Id, group.Code, group.Specialization.Name,
                DateTime.Now.Year - group.AdmissionYear.FirstYear, group.Students.Count), StatusCode.Ok, true,
            ResponseMessages.Success);
    }

    public async Task<DeleteGroupsResponse> DeleteAsync(string id)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
            );

        if (group is null)
        {
            return new DeleteGroupsResponse(StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        return await groupRepository.DeleteAsync(group)
            ? new DeleteGroupsResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new DeleteGroupsResponse(StatusCode.InternalServerError, false, ResponseMessages.Failed);
    }

    public async Task<UpdateGroupsResponse> UpdateAsync(string id, UpdateGroupRequest request)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
                        .Include(e => e.AdmissionYear)
            );

        if (group is null)
        {
            return new UpdateGroupsResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        group.SpecializationId = request.SpecialisationId;
        group.Code = request.GroupCode;
        group.AdmissionYear.FirstYear = request.Year;
        group.AdmissionYear.SecondYear += 1;

        return await groupRepository.UpdateAsync(group)
            ? new UpdateGroupsResponse(group.Id, StatusCode.Ok, true, ResponseMessages.Success)
            : new UpdateGroupsResponse(null, StatusCode.InternalServerError, false, ResponseMessages.Failed);
    }
}