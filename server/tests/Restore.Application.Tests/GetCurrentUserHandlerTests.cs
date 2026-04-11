using Restore.Application.Abstractions.Authentication;
using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class GetCurrentUserHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserLookupFails_ReturnsFailure()
    {
        var userRepository = new StubUserRepository
        {
            GetUserResult = Result<User>.Failure("user not found")
        };
        var tokenService = new StubTokenService();
        var handler = new GetCurrentUserHandler(userRepository, tokenService);

        var result = await handler.Handle(new GetCurrentUserQuery("john"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("user not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsTokenizedUserResponse()
    {
        var userRepository = new StubUserRepository
        {
            GetUserResult = Result<User>.Success(new User
            {
                UserName = "john",
                Email = "john@restore.dev"
            })
        };
        var tokenService = new StubTokenService { Token = "token-123" };
        var handler = new GetCurrentUserHandler(userRepository, tokenService);

        var result = await handler.Handle(new GetCurrentUserQuery("john"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("john", result.Value.Username);
        Assert.Equal("john@restore.dev", result.Value.Email);
        Assert.Equal("token-123", result.Value.Token);
    }

    private sealed class StubTokenService : ITokenService
    {
        public string Token { get; set; } = "token";

        public Task<string> GenerateToken(User user) => Task.FromResult(Token);
    }

    private sealed class StubUserRepository : IUserRepository
    {
        public Result<User> GetUserResult { get; set; } = Result<User>.Failure("not used");

        public Task<Result<User>> GetUserAsync(string username) => Task.FromResult(GetUserResult);
        public Task<Result<User>> LoginAsync(string username, string password) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<Result<User>> RegisterAsync(string username, string password, string email) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<User?> ReadAsync(string username) => Task.FromResult<User?>(null);
        public Task<User?> ReadUserAddressAsync(string username) => Task.FromResult<User?>(null);
        public Task UpdateAsync(User user) => Task.CompletedTask;
    }
}