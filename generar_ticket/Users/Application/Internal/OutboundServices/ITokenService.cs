using generar_ticket.Users.Domain.Model.Aggregate;

namespace generar_ticket.Users.Application.Internal.OutboundServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Task<int?> ValidateToken(string token);
        void InvalidateToken(string token);
        Task<int?> ValidateRefreshToken(string refreshToken);
        string GenerateRefreshToken();
        void StoreRefreshToken(string refreshToken, User user);
    }
}