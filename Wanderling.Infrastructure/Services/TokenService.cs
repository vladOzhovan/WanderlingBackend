using System.Text;
using Wanderling.Application.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wanderling.Application.Interfaces;
using Wanderling.Infrastructure.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Wanderling.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwt;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IOptions<JwtOptions> jwt)
        {
            _jwt = jwt.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecurityKey));
        }

        public string GenerateToken(TokenDto dto)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, dto.UserId),
                new(JwtRegisteredClaimNames.Email, dto.Email),
                new(JwtRegisteredClaimNames.UniqueName, dto.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in dto.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken
            (
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenLifetimeMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
