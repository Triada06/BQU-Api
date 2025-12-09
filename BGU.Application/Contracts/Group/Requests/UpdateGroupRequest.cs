namespace BGU.Application.Contracts.Group.Requests;

public record UpdateGroupRequest(string GroupCode, string SpecialisationId, int Year);