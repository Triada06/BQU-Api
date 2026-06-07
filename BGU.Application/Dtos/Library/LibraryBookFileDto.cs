namespace BGU.Application.Dtos.Library;

public sealed record LibraryBookFileDto(
    string FullPath,
    string ContentType,
    string FileName);
