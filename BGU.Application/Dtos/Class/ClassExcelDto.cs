using BGU.Core.Enums;

namespace BGU.Application.Dtos.Class;

public record ClassExcelDto(string? Id, ClassType ClassType, string TaughtSubjectId, string ClassTimeId, string Operation);
