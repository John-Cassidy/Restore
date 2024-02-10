using Restore.Core.Results;

namespace Restore.Application.Services;

public interface IImageService
{
    Task<Result<string>> AddImageAsync(IFormFileService formFileService);
}
