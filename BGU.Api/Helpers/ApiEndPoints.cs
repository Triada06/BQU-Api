namespace BGU.Api.Helpers;

public static class ApiEndPoints
{
    private const string ApiBase = "api";

    public static class User
    {
        private const string Base = $"{ApiBase}/user";
        public const string SignIn = $"{Base}/signin";
        public const string SignUp = $"{Base}/signup";
        public const string DeanSignUp = $"{Base}/dean-signup";
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
        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Syllabus
    {
        private const string Base = $"{ApiBase}/syllabus";
        public const string Create = $"{Base}/{{taughtSubjectId}}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Room
    {
        private const string Base = $"{ApiBase}/rooms";
        public const string Create = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Admin
    {
        private const string Base = $"{ApiBase}/admin";
    }

    public static class Student
    {
        private const string Base = $"{ApiBase}/students";
        public const string Create = $"{Base}";
        public const string Filter = $"{Base}/filter-by";
        public const string Search = $"{Base}/search";
        public const string GetAll = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
        public const string DashBoard = $"{Base}/dashboard";
        public const string Profile = $"{Base}/profile";
        public const string Grades = $"{Base}/grades/{{grade}}";
        public const string Schedule = $"{Base}/schedule/{{schedule}}";
        public const string MarkAbsence = $"{Base}/{{studentId}}/classes/{{classId}}/mark-absence";
        public const string GradeSeminar = $"{Base}/{{studentId}}/seminars/{{seminarId}}/grade";
        public const string GradeFinal = $"{Base}/{{studentId}}/finals/{{finalId}}/grade";
        public const string GradeColloquium = $"{Base}/{{studentId}}/colloquiums/{{colloquiumId}}/grade";

        public const string GradeIndependentWork =
            $"{Base}/{{studentId}}/independent-works/{{independentWorkId}}/grade";
    }

    public static class AdmissionYear
    {
        private const string Base = $"{ApiBase}/admission-years";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class Faculty
    {
        private const string Base = $"{ApiBase}/faculties";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class Department
    {
        private const string Base = $"{ApiBase}/departments";
        public const string GetAll = $"{ApiBase}/departments";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class Specialization
    {
        private const string Base = $"{ApiBase}/specializations";
        public const string GetAll = $"{ApiBase}/specializations";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class Group
    {
        private const string Base = $"{ApiBase}/groups";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class Subject
    {
        private const string Base = $"{ApiBase}/subjects";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class LectureHall
    {
        private const string Base = $"{ApiBase}/lecture-halls";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class ClassTime
    {
        private const string Base = $"{ApiBase}/class-time";
        public const string Create = $"{Base}/";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
    }

    public static class TaughtSubject
    {
        private const string Base = $"{ApiBase}/taught-subjects";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
        public const string GetAllColloquiums = $"{Base}/{{taughtSubjectId}}/colloquiums";
        
    }

    public static class Teacher
    {
        private const string Base = $"{ApiBase}/teachers";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
        public const string Profile = $"{Base}/profile";
        public const string MyCourses = $"{Base}/courses";
        public const string Schedule = $"{Base}/schedule/{{schedule}}";
    }

    public static class Dean
    {
        private const string Base = $"{ApiBase}/deans";
        public const string Create = $"{Base}";
        public const string Template = $"{Base}/template";
        public const string Import = $"{Base}/import";
        public const string Profile = $"{Base}/profile";
        public const string MyCourses = $"{Base}/courses";
        public const string Schedule = $"{Base}/schedule/{{schedule}}";
    }

    public static class Colloquium
    {
        private const string Base = $"{ApiBase}/colloquiums";
        public const string Create = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";
    }
}