using Restore.Core.Entities;

namespace Restore.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateToken(User user);
}
