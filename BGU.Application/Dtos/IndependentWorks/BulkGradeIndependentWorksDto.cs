using BGU.Application.Contracts.IndependentWorks.Requests;

namespace BGU.Application.Dtos.IndependentWorks;

public sealed record BulkGradeIndependentWorksDto(
    List<BulkGradeIndependentWorkRequest> IndependentWorkRequests);