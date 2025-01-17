using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Queries;
using generar_ticket.ticket.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.ticket.Domain.Services;

namespace generar_ticket.ticket.Application.Services
{
    public class TicketQueryService : ITicketQueryService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketQueryService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<Ticket?> Handle(GetTicketByIdQuery query)
        {
            return await _ticketRepository.GetTicketByIdAsync(query);
        }

        public async Task<Ticket?> Handle(GetTicketByNumberQuery query)
        {
            return await _ticketRepository.GetTicketByNumberAsync(query);
        }

        public async Task<IEnumerable<Ticket>> Handle(GetAllTicketQuery query)
        {
            return await _ticketRepository.ListAsync();
        }
    }
}