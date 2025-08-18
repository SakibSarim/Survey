using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TsrmWebApi.Security.IAuthService;
using TsrmWebApi.Security.Models.PresentationModels;
using System.Security.Cryptography;

namespace TsrmWebApi.Security.AuthService
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expireHours;

        public TokenGenerator(IOptions<JwtSettings> settings)
        {
            _key = settings.Value.Key;
            _issuer = settings.Value.Issuer;
            _audience = settings.Value.Audience;
            _expireHours = settings.Value.ExpireHours;
        }



        public Task<AuthDTO> IidentityToken(string userName, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_key);

            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddDays(60);

            // 1. Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, userId)
        }),
                Expires = expiresAt,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // 2. Create token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // 3. Generate Refresh Token
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            // 4. Return full token payload
            var authDto = new AuthDTO
            {
                Token = tokenString,
                RefreshToken = refreshToken,
                Success = true,
                expires_in = (int)(expiresAt - issuedAt).TotalSeconds, // <-- in seconds
                ActionTime = issuedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return Task.FromResult(authDto);
        }

        public Task<TokenResponseModel> IidentityTokenRefresh(string userName, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);
            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddMinutes(30);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, userName),
            new Claim("UserId", userId)
        }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString(); // You can later store this in DB or cache

            return Task.FromResult(new TokenResponseModel
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            });
        }


        public Task<bool> ValidateRefreshToken(string refreshToken, string userName)
        {
            // For now, accept any non-empty refresh token as valid (for demo)
            // You can improve this to validate against a DB or Redis cache
            bool isValid = !string.IsNullOrEmpty(refreshToken) && !string.IsNullOrEmpty(userName);
            return Task.FromResult(isValid);
        }

    }
}
