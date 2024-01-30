using Microsoft.AspNetCore.Identity;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<User>> GetUserAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return Result<User>.Failure("User not found");

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return Result<User>.Failure("Unauthorized");

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> RegisterAsync(string username, string password, string email)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user != null)
            return Result<User>.Failure("username: Username already exists");

        user = new User
        {
            UserName = username,
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            IEnumerable<string>? errors = result.Errors.Select((error, index) => $"{(string)("password" + index)}: {error.Description}");
            return Result<User>.Failure(string.Join(", ", errors));
        }

        await _userManager.AddToRoleAsync(user, "Member");

        return Result<User>.Success(user);
    }
}
