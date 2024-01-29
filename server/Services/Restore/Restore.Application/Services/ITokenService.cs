using Restore.Core.Entities;

namespace Restore.Application.Abstractions.Authentication;

public interface ITokenService
{
    Task<string> GenerateToken(User user);
}
