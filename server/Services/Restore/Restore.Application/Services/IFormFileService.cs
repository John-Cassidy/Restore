namespace Restore.Application.Services;

public interface IFormFileService
{
    string FileName { get; }
    Stream OpenReadStream();
}
