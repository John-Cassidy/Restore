using Restore.Application.Services;
using Restore.Core.Results;

namespace Restore.Infrastructure.Services;

public class ImageService : IImageService
{
    public async Task<Result<string>> AddImageAsync(IFormFileService formFileService)
    {
        var file = formFileService;
        if (file.FileName == null && file.FileName?.Length == 0) return Result<string>.Failure("File is required");

        var folderName = Path.Combine("images", "products");

        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

        // check if folder exists
        if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

        var fileName = file.FileName!;
        var fullPath = Path.Combine(pathToSave, fileName);
        var dbPath = Path.Combine("/images/products/", fileName);

        if (File.Exists(fullPath)) return Result<string>.Failure("File already exists");

        // use file.OpenReadStream() to get the file stream and then save it to the server using the fullPath
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.OpenReadStream().CopyToAsync(stream);
        }


        return Result<string>.Success(dbPath);
    }

    public async Task<Result<string>> UpdateImageAsync(IFormFileService formFileService, string pictureUrl)
    {
        var file = formFileService;
        if (file.FileName == null && file.FileName?.Length == 0) return Result<string>.Failure("File is required");

        var folderName = Path.Combine("images", "products");

        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

        // check if folder exists
        if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

        var fileName = file.FileName!;
        var fullPath = Path.Combine(pathToSave, fileName);
        var dbPath = Path.Combine("/images/products/", fileName);

        var oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pictureUrl.TrimStart('/'));
        var areFileNamesEqual = fileName == pictureUrl.TrimStart('/').Split('/').Last();
        if (File.Exists(fullPath) && areFileNamesEqual) File.Delete(fullPath);
        else if (File.Exists(fullPath) && !areFileNamesEqual) return Result<string>.Failure("File already exists");

        // use file.OpenReadStream() to get the file stream and then save it to the server using the fullPath
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.OpenReadStream().CopyToAsync(stream);
        }

        return Result<string>.Success(dbPath);
    }

    public async Task<Result<bool>> DeleteImageAsync(string imagePath)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

        if (File.Exists(fullPath))
        {
            await Task.Run(() => File.Delete(fullPath));
        }
        return Result<bool>.Success(true);
    }


}
