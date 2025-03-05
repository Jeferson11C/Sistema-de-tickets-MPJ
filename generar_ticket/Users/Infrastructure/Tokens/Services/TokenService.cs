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
using System.Collections.Concurrent;
using System.Security.Cryptography;
using generar_ticket.Users.Domain.Repositories;
using generar_ticket.Users.Interface.REST;

namespace generar_ticket.Users.Infrastructure.Tokens.Services
{

    namespace generar_ticket.Users.Infrastructure.Tokens.Services
    {
        public class TokenService : ITokenService
        {
            private readonly TokenSettings _tokenSettings;
            private readonly IUserRepository _userRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            private static readonly ConcurrentDictionary<string, bool> InvalidTokens =
                new ConcurrentDictionary<string, bool>();

            private static readonly ConcurrentDictionary<string, int> RefreshTokens =
                new ConcurrentDictionary<string, int>();

            public TokenService(IOptions<TokenSettings> tokenSettings, IUserRepository userRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _tokenSettings = tokenSettings.Value;
                _userRepository = userRepository;
                _httpContextAccessor = httpContextAccessor;
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
                        new Claim("Ventanilla", user.Ventanilla),
                        new Claim(ClaimTypes.Role, user.Rol), // Add role claim
                        new Claim("Area", user.Area) // Add area claim
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

            public async Task<int?> ValidateToken(string token)
            {
                if (string.IsNullOrEmpty(token) || InvalidTokens.ContainsKey(token)) return null;
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
                    var principal =
                        tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                    var userId = int.Parse(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)
                        .Value);

                    // Check if the token is expired
                    if (validatedToken.ValidTo < DateTime.UtcNow)
                    {
                        InvalidateToken(token); // Invalidate the token if it is expired
                        await SignOutAsync(); // Call SignOut method to close the session
                        return null; // Return null to indicate the token is no longer valid
                    }

                    return userId;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }

            private async Task SignOutAsync()
            {
                // Implement the logic to sign out the user and close the session
                // This could involve removing cookies, clearing session data, etc.
                Console.WriteLine("User signed out due to expired token.");
                var context = _httpContextAccessor.HttpContext;
                var controller = (UserController)context.RequestServices.GetService(typeof(UserController));
                controller.SignOut();
            }

            public string GenerateRefreshToken()
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }

            public void InvalidateToken(string token)
            {
                InvalidTokens[token] = true;
            }

            public async Task<int?> ValidateRefreshToken(string refreshToken)
            {
                if (RefreshTokens.TryGetValue(refreshToken, out var userId))
                {
                    return await Task.FromResult(userId);
                }

                return await Task.FromResult<int?>(null);
            }
        }
    }
}
