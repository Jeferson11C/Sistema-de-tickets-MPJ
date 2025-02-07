using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;

namespace generar_ticket.ticket.Domain.Services
{
    public interface ITicketCommandService
    {
        Task<Ticket> Handle(CreateTicketCommand command, Area area);
        Task<bool> Handle(UpdateTicketStatusCommand command); // Add this line
    }
}