using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Model.Aggregates;
using System.Threading.Tasks;

namespace generar_ticket.ticket.Domain.Services
{
    public interface ITicketCommandService
    {
        Task<Ticket?> Handle(CreateTicketCommand command);
        Task UpdateTicketAsync(Ticket ticket);
    }
}