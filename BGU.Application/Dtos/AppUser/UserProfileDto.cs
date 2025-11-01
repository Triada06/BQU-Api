namespace BGU.Application.Dtos.AppUser;

public record UserProfileDto(
    string Id,
    string FirstName,
    string LastName,
    double Gpa,
    int AdmissionYear,
    string Faculty,
    string Specialization,
    string Email,
    DateTime Birthday);