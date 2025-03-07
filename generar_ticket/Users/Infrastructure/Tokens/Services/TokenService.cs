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
using generar_ticket.Users.Domain.Model.Entity;
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

            private static readonly ConcurrentDictionary<string, (int UserId, DateTime Expiration)> RefreshTokens =
                new ConcurrentDictionary<string, (int UserId, DateTime Expiration)>();

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
                    Expires = DateTime.UtcNow.AddMinutes(30),
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
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            var userId = int.Parse(principal.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            // Check if the token is expired
            if (validatedToken.ValidTo < DateTime.UtcNow)
            {
                // Call the RefreshToken method instead of signing out
                var newToken = await RefreshToken(token);
                if (newToken == null)
                {
                    InvalidateToken(token); // Invalidate the token if refresh fails
                    return null; // Return null to indicate the token is no longer valid
                }

                return userId;
            }

            return userId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private async Task<string> RefreshToken(string expiredToken)
    {
        var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return null;

        var userId = await ValidateRefreshToken(refreshToken);
        if (userId == null) return null;

        var user = await _userRepository.GetUserByIdAsync(userId.Value);
        if (user == null) return null;

        var newToken = GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();

        // Store the new refresh token in an HttpOnly cookie
        StoreRefreshToken(newRefreshToken, user);

        // Update the response headers with the new token
        _httpContextAccessor.HttpContext.Response.Headers["Token"] = newToken;

        return newToken;
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

    public void StoreRefreshToken(string refreshToken, User user)
    {
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = !string.IsNullOrEmpty(refreshToken)
                ? DateTime.UtcNow.AddDays(7)
                : (DateTime?)null;
            _userRepository.UpdateAsync(user).Wait();
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Set to true if using HTTPS
            Expires = DateTime.UtcNow.AddDays(7)
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
    }

    public void InvalidateToken(string token)
    {
        InvalidTokens[token] = true;
    }

    public async Task<int?> ValidateRefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return null;
        }
        return user.Id;
            }
        }
    }
}
