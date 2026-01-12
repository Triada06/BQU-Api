namespace BGU.Application.Common;

public class BulkImportResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string? TemporaryPassword { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
}