using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Interfaces.REST.Resources;

namespace generar_ticket.ticket.Interfaces.REST.Transform
{
    public static class CreateTicketCommandFromResourceAssembler
    {
        public static CreateTicketCommand ToCommandFromResource(CreatedTicketResource resource)
        {
            return new CreateTicketCommand(
                resource.TicketNumber,
                resource.Status,
                resource.CreatedDate,
                resource.UpdatedDate,
                resource.UserName.FirstName,
                resource.UserName.LastName
            );
        }
    }
}