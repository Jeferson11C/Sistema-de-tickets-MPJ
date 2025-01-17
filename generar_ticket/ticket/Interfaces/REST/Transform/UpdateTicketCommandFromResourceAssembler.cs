using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Interfaces.REST.Resources;

namespace generar_ticket.ticket.Interfaces.REST.Transform
{
    public static class UpdateTicketCommandFromResourceAssembler
    {
        public static UpdateTicketCommand ToCommandFromResource(UpdatedTicketResource resource)
        {
            return new UpdateTicketCommand(
                resource.Id,
                resource.Status
            );
        }
    }
}