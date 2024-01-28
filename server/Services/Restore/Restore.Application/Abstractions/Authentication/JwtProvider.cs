namespace Restore.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    public string GenerateJwtToken(string userId, string userName, string secret);
}
