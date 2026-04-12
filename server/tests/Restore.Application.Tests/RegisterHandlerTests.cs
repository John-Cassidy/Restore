using MediatR;
using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class RegisterHandlerTests
{
    [Fact]
    public async Task Handle_WhenRegistrationFails_ReturnsFailure()
    {
        var userRepository = new StubUserRepository
        {
            RegisterResult = Result<User>.Failure("Username already taken")
        };
        var handler = new RegisterHandler(userRepository);

        var result = await handler.Handle(
            new RegisterCommand("john", "P@ss1234", "john@restore.dev"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Username already taken", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenRegistrationSucceeds_ReturnsSuccessUnit()
    {
        var userRepository = new StubUserRepository
        {
            RegisterResult = Result<User>.Success(new User { UserName = "john" })
        };
        var handler = new RegisterHandler(userRepository);

        var result = await handler.Handle(
            new RegisterCommand("john", "P@ss1234", "john@restore.dev"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);
    }

    private sealed class StubUserRepository : IUserRepository
    {
        public Result<User> RegisterResult { get; set; } = Result<User>.Failure("not configured");

        public Task<Result<User>> RegisterAsync(string username, string password, string email) =>
            Task.FromResult(RegisterResult);

        public Task<Result<User>> LoginAsync(string username, string password) =>
            Task.FromResult(Result<User>.Failure("not used"));

        public Task<Result<User>> GetUserAsync(string username) =>
            Task.FromResult(Result<User>.Failure("not used"));

        public Task<User?> ReadAsync(string username) => Task.FromResult<User?>(null);
        public Task<User?> ReadUserAddressAsync(string username) => Task.FromResult<User?>(null);
        public Task UpdateAsync(User user) => Task.CompletedTask;
    }
}
