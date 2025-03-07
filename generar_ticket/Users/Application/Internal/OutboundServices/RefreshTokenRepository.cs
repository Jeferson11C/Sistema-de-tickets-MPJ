using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.Users.Domain.Model.Entity;
using generar_ticket.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace generar_ticket.Users.Application.Internal.OutboundServices
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
        

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task RemoveAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}