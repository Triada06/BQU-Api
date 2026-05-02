namespace BGU.Api.Helpers;

public static class ApiEndPoints
{
    private const string ApiBase = "api";

    public static class User
    {
        private const string Base = $"{ApiBase}/user";
        public const string SignIn = $"{Base}/signin";
        public const string ChangePassword = $"{Base}/me/change-password";
        public const string CheckPassword = $"{Base}/check-password";
        public const string AddEmail = $"{Base}/me/add-email";
        public const string ConfirmEmail = $"{Base}/me/confirm-email";
    }

    public static class Auth
    {
        private const string Base = $"{ApiBase}/auth";
        public const string ForgotPassword = $"{Base}/forgot-password";
        public const string ResetPassword = $"{Base}/reset-password";
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

    public static class Student
    {
        private const string Base = $"{ApiBase}/students";
        public const string Create = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Filter = $"{Base}/filter-by";
        public const string Search = $"{Base}/search";
        public const string GetAll = $"{Base}";
        public const string Import = $"{Base}/import";
        public const string DashBoard = $"{Base}/dashboard";
        public const string Profile = $"{Base}/profile";
        public const string Grades = $"{Base}/me/grades";
        public const string Schedule = $"{Base}/schedule/{{schedule}}";
        public const string MarkAbsence = $"{Base}/{{studentId}}/classes/{{classId}}/mark-absence";
        public const string GradeColloquium = $"{Base}/{{studentId}}/colloquiums/{{colloquiumId}}/grade";

        public const string GetIndependentWorksByStudentId =
            $"{Base}/{{studentId}}/taught-subjects/{{taughtSubjectId}}/independent-works";

        public const string AcademicHistory = $"{Base}/me/academic-history";
        public const string GetFinals = $"{Base}/me/finals";
        public const string ResetPassword = $"{Base}/{{id}}/reset-password";
    }

    public static class Department
    {
        public const string GetAll = $"{ApiBase}/departments";
    }

    public static class Specialization
    {
        public const string GetAll = $"{ApiBase}/specializations";
    }

    public static class Group
    {
        private const string Base = $"{ApiBase}/groups";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string Schedule = $"{Base}/{{id}}/schedule";
        public const string Delete = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
        public const string SetExamDate = $"{Base}/set-exam-date";
    }

    public static class ClassTime
    {
        private const string Base = $"{ApiBase}/class-time";
        public const string Create = $"{Base}/";
    }

    public static class TaughtSubject
    {
        private const string Base = $"{ApiBase}/taught-subjects";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string GetSyllabus = $"{Base}/{{id}}/syllabus";
        public const string RemoveSyllabus = $"{Base}/{{id}}/syllabus";
        public const string GetAllColloquiums = $"{Base}/{{taughtSubjectId}}/colloquiums";
        public const string GetStudentsAndAttendances = $"{Base}/{{taughtSubjectId}}/classes";
        public const string GetStudentsInSubject = $"{Base}/{{taughtSubjectId}}/students";
        public const string GetIndependentWorks = $"{Base}/{{taughtSubjectId}}/IndependendWorks";
    }

    public static class Teacher
    {
        private const string Base = $"{ApiBase}/teachers";
        public const string GetAll = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Import = $"{Base}/import";
        public const string Profile = $"{Base}/profile";
        public const string MyCourses = $"{Base}/courses";
        public const string Schedule = $"{Base}/schedule/{{schedule}}";
        public const string Exams = $"{Base}/me/exams";
    }

    public static class Dean
    {
        private const string Base = $"{ApiBase}/deans";
        public const string Profile = $"{Base}/profile";
    }

    public static class Colloquium
    {
        private const string Base = $"{ApiBase}/colloquiums";
        public const string Create = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class IndependentWork
    {
        private const string Base = $"{ApiBase}/independent-works";
        public const string Create = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";

        public const string Grade =
            $"{Base}/{{id}}/grade";
    }

    public static class Seminar
    {
        private const string Base = $"{ApiBase}/seminars";
        public const string GetAll = $"{Base}";
        public const string Create = $"{Base}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GradeSeminar = $"{Base}/{{seminarId}}/grade";
    }

    public static class StudentSubjectEnrollments
    {
        private const string Base = $"{ApiBase}/student-subject-enrollments";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string GetById = $"{Base}/{{id}}";
    }
    public static class Finals
    {
        private const string Base = $"{ApiBase}/finals";
        public const string Create = $"{Base}";
        public const string GetAll = $"{Base}";
        public const string SetTime = $"{Base}/{{id}}/set-time";
        public const string Update = $"{Base}/{{id}}";
        public const string Confirm = $"{Base}/{{id}}/confirm";
        public const string Grade = $"{Base}/{{id}}/grade";
        
    }
}