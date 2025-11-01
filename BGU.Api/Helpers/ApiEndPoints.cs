namespace BGU.Api.Helpers;

public static class ApiEndPoints
{
    private const string ApiBase = "api";

    public static class User
    {
        private const string Base = $"{ApiBase}/user";
        public const string SignIn = $"{Base}/signin";
        public const string SignUp = $"{Base}/signup";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string VerifyPassword = $"{Base}/me/verify-password";
        public const string ChangePassword = $"{Base}/me/security/change-password";
        public const string ConfirmEmail = $"{Base}/{{userId}}/confirm-email";
        public const string Profile = $"{Base}/me";
    }

    public static class Class
    {
        private const string Base = $"{ApiBase}/class";
        public const string Create = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }
}