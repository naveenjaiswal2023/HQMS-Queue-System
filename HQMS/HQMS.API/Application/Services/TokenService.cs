
using HQMS.API.Domain.Entities;
using HQMS.Application.DTO;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using Jose;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HQMS.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtDto _jwt;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public TokenService(
        IOptions<JwtDto> jwtOptions,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
        {
            _jwt = jwtOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<TokenDto> GenerateToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleName = userRoles.FirstOrDefault() ?? string.Empty;

            // Get the RoleId for the first role
            var role = await _roleManager.FindByNameAsync(roleName);
            var roleId = role?.Id ?? string.Empty;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("RoleId", roleId) // Optional: Add roleId as a custom claim
            };

            foreach (var uRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, uRole));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Optional: generate refresh token here if needed
            //var refreshToken = GenerateRefreshToken();

            return new TokenDto
            {
                Token = tokenString,
                Expiration = expires,
                UserId = user.Id,
                Role = userRoles.FirstOrDefault() ?? string.Empty,
                RoleId = roleId // Add this to your DTO
                //RefreshToken = refreshToken
            };
        }

    }

}
