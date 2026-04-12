using Restore.Infrastructure.Authentication;

namespace Restore.Infrastructure.Tests;

public class JwtOptionsTests
{
    [Fact]
    public void InitProperties_WhenSet_AreAvailable()
    {
        var options = new JwtOptions
        {
            Issuer = "restore-api",
            Audience = "restore-client",
            SecretKey = "test-secret-key"
        };

        Assert.Equal("restore-api", options.Issuer);
        Assert.Equal("restore-client", options.Audience);
        Assert.Equal("test-secret-key", options.SecretKey);
    }
}