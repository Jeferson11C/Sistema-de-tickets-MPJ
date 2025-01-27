using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Model.Queries;
using generar_ticket.ticket.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace generar_ticket.ticket.Application.Internal.QueryServices
{
    public class TicketQueryService : ITicketQueryService
    {
        private readonly AppDbContext _context;

        public TicketQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> Handle(GetAllTicketsQuery query)
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<object?> Handle(GetTicketByIdQuery query)
        {
            return await _context.Tickets.FindAsync(query.Id);
        }

        public async Task<IEnumerable<Ticket>> Handle(GetTicketsByAreaQuery query)
        {
            return await _context.Tickets.Where(t => t.AreaNombre == query.AreaNombre).ToListAsync();
        }

        public async Task<bool> Handle(int id, UpdateTicketStatusCommand command) // Implement this method
        {
            var ticket = await _context.Tickets.SingleOrDefaultAsync(t => t.Id == id);
            if (ticket == null)
            {
                return false;
            }

            ticket.Estado = command.Estado;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}