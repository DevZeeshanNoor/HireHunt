using HireHuntBackend.Common.Dto;
using HireHuntBackend.Context;
using HireHuntBackend.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HireHuntBackend.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly DbContextHireHunt _context;
        public JwtTokenService(IConfiguration configuration, DbContextHireHunt context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<TokenResponseDto> GenerateToken(string userId, string email,string role)
        {

            var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // 2️⃣ Create secret key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            // 3️⃣ Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4️⃣ Create JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])
                ),
                signingCredentials: creds
            );
            //Refresh Token
            var refreshToken = GenerateRefreshToken();
            var rf = new RefreshToken()
            {
                UserId = Convert.ToInt32(userId),
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _context.RefreshTokens.AddAsync(rf);
            await  _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken
            };

        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

    }
}
