using System.Threading.Tasks;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Entity;

namespace generar_ticket.Users.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RemoveAsync(RefreshToken refreshToken);
    }
}