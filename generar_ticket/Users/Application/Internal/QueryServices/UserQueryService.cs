using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Queries;
using generar_ticket.Users.Domain.Services;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace generar_ticket.Users.Application.Internal.QueryServices
{
    public class UserQueryService : IUserQueryService
    {
        private readonly AppDbContext _context;

        public UserQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .ToListAsync();
        }

        public async Task<User?> Handle(GetUserByIdQuery query)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .FirstOrDefaultAsync(u => u.Id == query.Id);
        }

        public async Task<IEnumerable<User>> Handle(GetUsersByRoleQuery query)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .Where(u => u.Rol == query.Rol)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> Handle(GetUsersByAreaQuery query)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .Where(u => u.Area == query.Area)
                .ToListAsync();
        }
    }
}