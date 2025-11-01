namespace BGU.Application.Contracts.User;

public record AuthResponse(
    string? Token,
    DateTime? ExpireTime,
    bool IsSucceeded,
    IEnumerable<string>? Errors);