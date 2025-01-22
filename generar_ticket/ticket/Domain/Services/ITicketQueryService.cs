using System.Threading.Tasks;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Model.Queries;

namespace generar_ticket.ticket.Domain.Services
{
    public interface ITicketQueryService
    {
        Task<IEnumerable<Ticket>> Handle(GetAllTicketsQuery query);
        Task<object?> Handle(GetTicketByIdQuery query);
        Task<IEnumerable<Ticket>> Handle(GetTicketsByAreaQuery query);
        Task<bool> Handle(int id, UpdateTicketStatusCommand command); // Update this line
    }
}