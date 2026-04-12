using Restore.Application.Abstractions.Authentication;
using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_WhenLoginFails_ReturnsFailure()
    {
        var userRepository = new StubUserRepository
        {
            LoginResult = Result<User>.Failure("Invalid credentials")
        };
        var tokenService = new StubTokenService();
        var handler = new LoginHandler(userRepository, tokenService);

        var result = await handler.Handle(new LoginCommand("john", "wrong"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
        Assert.False(tokenService.GenerateCalled);
    }

    [Fact]
    public async Task Handle_WhenLoginSucceeds_ReturnsUserResponseWithToken()
    {
        var userRepository = new StubUserRepository
        {
            LoginResult = Result<User>.Success(new User
            {
                UserName = "john",
                Email = "john@restore.dev"
            })
        };
        var tokenService = new StubTokenService { Token = "jwt-abc-123" };
        var handler = new LoginHandler(userRepository, tokenService);

        var result = await handler.Handle(new LoginCommand("john", "correct"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("john", result.Value.Username);
        Assert.Equal("john@restore.dev", result.Value.Email);
        Assert.Equal("jwt-abc-123", result.Value.Token);
        Assert.True(tokenService.GenerateCalled);
    }

    private sealed class StubTokenService : ITokenService
    {
        public string Token { get; set; } = "default-token";
        public bool GenerateCalled { get; private set; }

        public Task<string> GenerateToken(User user)
        {
            GenerateCalled = true;
            return Task.FromResult(Token);
        }
    }

    private sealed class StubUserRepository : IUserRepository
    {
        public Result<User> LoginResult { get; set; } = Result<User>.Failure("not configured");

        public Task<Result<User>> LoginAsync(string username, string password) => Task.FromResult(LoginResult);
        public Task<Result<User>> GetUserAsync(string username) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<Result<User>> RegisterAsync(string username, string password, string email) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<User?> ReadAsync(string username) => Task.FromResult<User?>(null);
        public Task<User?> ReadUserAddressAsync(string username) => Task.FromResult<User?>(null);
        public Task UpdateAsync(User user) => Task.CompletedTask;
    }
}
