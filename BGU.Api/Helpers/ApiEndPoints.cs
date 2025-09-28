namespace BGU.Api.Helpers;

public static class ApiEndPoints
{
    private const string ApiBase = "api";

    public static class AppUser
    {
        private const string Base = $"{ApiBase}/appuser";
        public const string SignIn = $"{Base}/signin";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string VerifyPassword = $"{Base}/me/verifypassword";
        public const string ChangePassword = $"{Base}/me/security/changepassword";
        public const string ConfirmEmail = $"{Base}/{{userId}}/confirmemail";
    }
}