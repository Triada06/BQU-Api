using BGU.Application.Common;

namespace BGU.Application.Services.Interfaces;

public interface ITranscriptService
{
    Task<ApiResult<byte[]>> GeneratePdfAsync(string studentId);
    Task<ApiResult<byte[]>> GenerateExcelAsync(string studentId);
}