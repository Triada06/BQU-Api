using System.Security.AccessControl;

namespace BGU.Application.Dtos.Student;

public sealed record GetActivitiesAndAttendances(List<GetActivityAndAttendance> ActivityAndAttendances);