using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Queries;

namespace generar_ticket.ticket.Domain.Services
{
    public class TicketQueryService : ITicketQueryService
    {
        private readonly List<Ticket> _tickets;

        public TicketQueryService()
        {
            _tickets = new List<Ticket>();
        }

        public Task<IEnumerable<Ticket>> Handle(GetAllTicketsQuery query)
        {
            return Task.FromResult(_tickets.AsEnumerable());
        }

        public Task<IEnumerable<Ticket>> Handle(GetTicketsByAreaQuery query)
        {
            var tickets = _tickets.Where(t => t.AreaNombre == query.AreaNombre);
            return Task.FromResult(tickets.AsEnumerable());
        }

        public Task<Ticket> Handle(GetTicketByNumberQuery query)
        {
            var ticket = _tickets.FirstOrDefault(t => t.NumeroTicket == query.NumeroTicket);
            return Task.FromResult(ticket);
        }

        public void AddTicket(Ticket ticket)
        {
            _tickets.Add(ticket);
        }
    }
}