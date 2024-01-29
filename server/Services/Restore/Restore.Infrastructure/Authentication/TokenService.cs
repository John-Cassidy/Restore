using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Restore.Application.Abstractions.Authentication;
using Restore.Core.Entities;

namespace Restore.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly UserManager<User> _userManager;

    public TokenService(UserManager<User> userManager, IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _userManager = userManager;
    }

    public async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
       issuer: null,
       audience: null,
       claims: claims,
       expires: DateTime.Now.AddDays(7),
       signingCredentials: credentials
   );

        // var tokenOptions = new JwtSecurityToken(
        //     _options.Issuer,
        //     _options.Audience,
        //     claims,
        //     null,
        //     DateTime.Now.AddMinutes(30),
        //     credentials
        // );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
