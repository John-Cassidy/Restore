using Restore.Core.Results;

namespace Restore.Application.Services;

public interface IImageService
{
    Task<Result<string>> AddImageAsync(IFormFileService formFileService);
    Task<Result<string>> UpdateImageAsync(IFormFileService formFileService, string pictureUrl);
    Task<Result<bool>> DeleteImageAsync(string imagePath);
}
