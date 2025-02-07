using generar_ticket.Users.Domain.Model.Aggregate;

namespace generar_ticket.Users.Application.Internal.OutboundServices
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Task<int?> ValidateToken(string token);
    }
}