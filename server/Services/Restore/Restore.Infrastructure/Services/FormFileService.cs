using Microsoft.AspNetCore.Http;
using Restore.Application.Services;

namespace Restore.Infrastructure.Services;

public class FormFileService : IFormFileService
{
    private readonly IFormFile _formFile;

    public FormFileService(IFormFile formFile)
    {
        _formFile = formFile;
    }

    public string FileName => _formFile.FileName;

    public Stream OpenReadStream() => _formFile.OpenReadStream();
}
