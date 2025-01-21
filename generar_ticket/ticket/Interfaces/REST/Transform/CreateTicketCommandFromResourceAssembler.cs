using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Interfaces.REST.Resources;

namespace generar_ticket.ticket.Interfaces.REST.Transform
{
    public static class CreateTicketCommandFromResourceAssembler
    {
        public static CreateTicketCommand ToCommand(CreatedTicketResource resource)
        {
            return new CreateTicketCommand(resource.Documento, resource.AreaNombre);
        }
    }
}