using generar_ticket.Shared.Domain.Repositories;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Queries;
using generar_ticket.ticket.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.ticket.Infrastructure.Persistence.EFC.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Ticket> _tickets;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
            _tickets = context.Set<Ticket>();
        }

        public async Task AddAsync(Ticket entity)
        {
            await _tickets.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Ticket?> FindByIdAsync(int id)
        {
            return await _tickets.FindAsync(id);
        }

        public async Task<Ticket?> FindByGroupAsync(string group)
        {
            // Implement logic to find by group if applicable
            return await Task.FromResult<Ticket?>(null);
        }

        public void Update(Ticket entity)
        {
            _tickets.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(Ticket entity)
        {
            _tickets.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Ticket>> ListAsync()
        {
            return await _tickets.ToListAsync();
        }

        public async Task<Ticket?> GetTicketByIdAsync(GetTicketByIdQuery query)
        {
            return await _tickets.FindAsync(query.Id);
        }

        public async Task<Ticket?> GetTicketByNumberAsync(GetTicketByNumberQuery query)
        {
            return await _tickets.FirstOrDefaultAsync(t => t.TicketNumber == query.TicketNumber);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }
}