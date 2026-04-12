using Microsoft.AspNetCore.Http;
using Restore.Application.Services;
using System.Diagnostics.CodeAnalysis;

namespace Restore.Infrastructure.Services;

[ExcludeFromCodeCoverage]
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
