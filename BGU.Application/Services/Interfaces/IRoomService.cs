using BGU.Application.Contracts.Rooms;
using BGU.Application.Contracts.Rooms.Requests;
using BGU.Application.Contracts.Rooms.Responses;

namespace BGU.Application.Services.Interfaces;

public interface IRoomService
{
    Task<GetAllRoomsResponse> GetAllAsync(int page, int pageSize, bool tracking = false);
    Task<CreateRoomResponse> CreateAsync(CreateRoomRequest request);
    Task<GetRoomByIdResponse> GetByIdAsync(string id, bool tracking = false);
    Task<UpdateRoomResponse> UpdateAsync(string id, UpdateRoomRequest request);
    Task<DeleteRoomResponse> DeleteAsync(string id);
}