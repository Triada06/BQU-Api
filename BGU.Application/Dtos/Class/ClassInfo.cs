using BGU.Core.Enums;

namespace BGU.Application.Dtos.Class;

public sealed record ClassInfo(DateTimeOffset Date, ClassType ClassType, bool IsAbsent, int? Score);