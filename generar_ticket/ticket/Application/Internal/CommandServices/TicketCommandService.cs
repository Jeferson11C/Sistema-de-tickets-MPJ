using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Repositories;
using System.Threading.Tasks;
using generar_ticket.ticket.Domain.Model.ValueObjects;
using generar_ticket.ticket.Domain.Services;

namespace generar_ticket.ticket.Application.Services
{
    public class TicketCommandService : ITicketCommandService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketCommandService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Ticket?> Handle(CreateTicketCommand command)
        {
            var userName = new UserName(command.FirstName, command.LastName);
            var ticket = Ticket.Create(command, userName);
            await _ticketRepository.AddAsync(ticket);
            return ticket;
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            await _ticketRepository.UpdateAsync(ticket);
        }
    }
}