namespace BGU.Application.Contracts.User;

public record ResetPasswordRequest(string UserId, string Token, string NewPassword);
