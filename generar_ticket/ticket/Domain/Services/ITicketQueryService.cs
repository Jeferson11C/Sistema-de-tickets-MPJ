using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace generar_ticket.ticket.Domain.Services
{
    public interface ITicketQueryService
    {
        Task<IEnumerable<Ticket>> Handle(GetAllTicketsQuery query);
        Task<IEnumerable<Ticket>> Handle(GetTicketsByAreaQuery query);
        Task<object?> Handle(GetTicketByIdQuery query);
    }
}