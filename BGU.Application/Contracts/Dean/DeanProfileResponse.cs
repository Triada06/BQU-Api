using BGU.Application.Dtos.Dean;

namespace BGU.Application.Contracts.Dean;

public sealed record DeanProfileResponse(
    DeanProfileDto? DeanProfile,
    string Message,
    bool IsSucceeded,
    int StatusCode);