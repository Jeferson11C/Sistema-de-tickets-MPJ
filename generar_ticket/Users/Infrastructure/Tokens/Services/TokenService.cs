using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using generar_ticket.Users.Application.Internal.OutboundServices;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Infrastructure.Tokens.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace generar_ticket.Users.Infrastructure.Tokens.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings _tokenSettings;

        public TokenService(IOptions<TokenSettings> tokenSettings)
        {
            _tokenSettings = tokenSettings.Value;
        }

        public string GenerateToken(User user)
        {
            var secret = _tokenSettings.Secret;
            if (string.IsNullOrEmpty(secret)) throw new Exception("Secret is not set");
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires =DateTime.UtcNow.AddMinutes(30), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<int?> ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _tokenSettings.Secret;
            if (string.IsNullOrEmpty(secret)) return null;
            var key = Encoding.ASCII.GetBytes(secret);
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                var userId = int.Parse(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
                return await Task.FromResult(userId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}