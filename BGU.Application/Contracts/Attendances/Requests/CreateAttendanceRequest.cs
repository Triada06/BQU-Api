namespace BGU.Application.Contracts.Attendances.Requests;

public sealed record CreateAttendanceRequest(string ClassId, string StudentId);