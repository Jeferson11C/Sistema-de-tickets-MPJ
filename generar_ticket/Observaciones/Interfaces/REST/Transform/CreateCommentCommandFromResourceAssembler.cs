using generar_ticket.Observaciones.Domain.Model.Commands;
using generar_ticket.Observaciones.Interfaces.REST.Resources;

namespace generar_ticket.Observaciones.Interfaces.REST.Transform
{
    public class CreateCommentCommandFromResourceAssembler
    {
        public CreateCommentCommand ToCommand(CreatedCommentResource resource)
        {
            return new CreateCommentCommand(
                resource.TicketId,
                resource.Coment,
                resource.UserId
            );
        }
    }
}