namespace BGU.Application.Contracts;

public class BulkImportResult
{
    public string Identifier { get; set; } // Email, Name, or Id
    public string Operation { get; set; } // CREATE, UPDATE, DELETE
    public bool Success { get; set; }
    public string Message { get; set; }
    public string EntityId { get; set; } // Created/Updated entity ID
}