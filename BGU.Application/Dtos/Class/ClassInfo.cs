using BGU.Core.Enums;

namespace BGU.Application.Dtos.Class;

public sealed record ClassInfo(DateTimeOffset Date, string ClassType, bool IsAbsent, int? Score);