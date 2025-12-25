using BGU.Core.Enums;

namespace BGU.Application.Contracts.Class.Requests;

public sealed record CreateClassRequest(ClassType ClassType, string TaughtSubjectId, Core.Entities.ClassTime  ClassTime);