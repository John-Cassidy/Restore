using MediatR;
using Microsoft.AspNetCore.Identity;
using Restore.Application.Abstractions.Authentication;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _jwtProvider;

    public UserRepository(UserManager<User> userManager, ITokenService jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
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
            return Result<User>.Failure("Username already exists");

        user = new User
        {
            UserName = username,
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description);
            return Result<User>.Failure("Validation Error(s): " + string.Join(", ", errors));
        }

        await _userManager.AddToRoleAsync(user, "Member");

        return Result<User>.Success(user);
    }
}
