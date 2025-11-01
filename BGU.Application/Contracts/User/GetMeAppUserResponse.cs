using BGU.Application.Dtos.AppUser;

namespace BGU.Application.Contracts.User;

public record GetMeAppUserResponse(UserProfileDto? UserProfileDto, string Message, bool IsSucceeded, int StatusCode);