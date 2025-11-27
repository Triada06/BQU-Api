using BGU.Application.Contracts.Rooms;
using BGU.Application.Contracts.Rooms.Requests;
using BGU.Application.Contracts.Rooms.Responses;
using BGU.Application.Dtos;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class RoomService(IRoomRepository roomRepository) : IRoomService
{
    public async Task<GetAllRoomsResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        var rooms = (await roomRepository.GetAllAsync(page, pageSize, tracking))?.Select(x =>
            new RoomDto(x.Id, x.Name, x.Capacity, x.RoomType.ToString()));
        return new GetAllRoomsResponse(rooms, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<CreateRoomResponse> CreateAsync(CreateRoomRequest request)
    {
        if (await roomRepository.AnyAsync(x => x.Name == request.RoomName))
            return new CreateRoomResponse(null, StatusCode.Conflict, false, ResponseMessages.AlreadyExists);

        var room = new Room
        {
            Name = request.RoomName,
            Capacity = request.Capacity,
            RoomType = request.RoomType
        };
        if (!await roomRepository.CreateAsync(room))
        {
            return new CreateRoomResponse(null, StatusCode.InternalServerError, false, ResponseMessages.Failed);
        }

        return new CreateRoomResponse(new RoomDto(room.Id, room.Name, room.Capacity, room.RoomType.ToString()),
            StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetRoomByIdResponse> GetByIdAsync(string id, bool tracking = false)
    {
        var room = await roomRepository.GetByIdAsync(id, tracking: tracking);
        return room is null
            ? new GetRoomByIdResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound)
            : new GetRoomByIdResponse(new RoomDto(room.Id, room.Name, room.Capacity, room.RoomType.ToString()),
                StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<UpdateRoomResponse> UpdateAsync(string id, UpdateRoomRequest request)
    {
        var room = await roomRepository.GetByIdAsync(id, tracking: true);

        if (room is null)
            return new UpdateRoomResponse(null, StatusCode.BadRequest, false, ResponseMessages.BadRequest);
        room.Name = request.Name;
        room.Capacity = request.Capacity;
        room.RoomType = request.RoomType;
        await roomRepository.UpdateAsync(room);
        return new UpdateRoomResponse(new RoomDto(room.Id, room.Name, room.Capacity, room.RoomType.ToString()),
            StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<DeleteRoomResponse> DeleteAsync(string id)
    {
        var room = await roomRepository.GetByIdAsync(id, tracking: false);

        if (room is null)
            return new DeleteRoomResponse(StatusCode.BadRequest, false, ResponseMessages.BadRequest);
        await roomRepository.DeleteAsync(room);
        return new DeleteRoomResponse(StatusCode.Ok, true, ResponseMessages.Success);
    }
}