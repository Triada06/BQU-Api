using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.User;

public record AuthResponse(
    string? Token,
    DateTime? ExpireTime,
    bool IsSucceeded,
    StatusCode StatusCode,
    IEnumerable<string>? Errors);