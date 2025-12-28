using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.IndependentWorks.Responses;

public sealed record DeleteIndependentWorkResponse(StatusCode StatusCode, bool IsSucceeded, string ResponseMessage);