using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Services;
using System.Threading.Tasks;
using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.ticket.Application.Internal.CommandServices
{
    public class TicketCommandService : ITicketCommandService
    {
        private readonly PersonaService _personaService;
        private readonly AppDbContext _context;

        public TicketCommandService(PersonaService personaService, AppDbContext context)
        {
            _personaService = personaService;
            _context = context;
        }

        public async Task<Ticket> Handle(CreateTicketCommand command)
        {
            var ticket = new Ticket(command, _personaService, _context);
            // Add logic to save the ticket to the repository if needed
            return await Task.FromResult(ticket);
        }

        public async Task<Ticket> Handle(CreateTicketCommand command, Area area)
        {
            var ticket = new Ticket(command, _personaService, _context)
            {
                AreaNombre = area.Nombre
            };
            // Add logic to save the ticket to the repository if needed
            return await Task.FromResult(ticket);
        }

        public Task<bool> Handle(UpdateTicketStatusCommand command)
        {
            throw new NotImplementedException();
        }
    }
}