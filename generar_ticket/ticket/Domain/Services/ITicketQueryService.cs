using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Queries;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace generar_ticket.ticket.Domain.Services
{
    public interface ITicketQueryService
    {
        Task<Ticket?> Handle(GetTicketByIdQuery query);
        Task<Ticket?> Handle(GetTicketByNumberQuery query);
        Task<IEnumerable<Ticket>> Handle(GetAllTicketQuery query);
    }
}