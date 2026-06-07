namespace BGU.Application.Dtos.Exams;

public sealed record BulkConfirmFinalsRequest(IEnumerable<string> Ids);
