namespace BGU.Application.Common;

public class BulkImportResult
{
    public string Identifier { get; set; }
    public string Operation { get; set; }
    public string? EntityId { get; set; }
    public string Email { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public string? TemporaryPassword { get; set; }
}