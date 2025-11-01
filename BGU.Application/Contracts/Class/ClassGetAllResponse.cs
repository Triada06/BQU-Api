using BGU.Application.Dtos.Class;

namespace BGU.Application.Contracts.Class;

public record ClassGetAllResponse(IEnumerable<ClassDto>? Dto, string Message, bool IsSucceeded, int StatusCode);