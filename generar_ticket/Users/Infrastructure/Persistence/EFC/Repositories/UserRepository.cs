using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Queries;
using generar_ticket.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace generar_ticket.Users.Infrastructure.Persistence.EFC.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .Where(u => u.Rol == role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByAreaAsync(string area)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .Where(u => u.Area == area)
                .ToListAsync();
        }

        public async Task<User> Handle(GetUserByIdQuery query)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .FirstOrDefaultAsync(u => u.Id == query.Id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<User?> FindByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> FindByGroupAsync(string group)
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .FirstOrDefaultAsync(u => u.Area == group);
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(User entity)
        {
            _context.Users.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _context.Users
                .Include(u => u.NombreCompleto)
                .ToListAsync();
        }
    }
}