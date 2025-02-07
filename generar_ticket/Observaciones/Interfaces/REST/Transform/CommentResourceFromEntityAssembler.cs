using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Observaciones.Interfaces.REST.Resources;

namespace generar_ticket.Observaciones.Interfaces.REST.Transform
{
    public class CommentResourceFromEntityAssembler
    {
        public CommentResources ToResource(Comment entity)
        {
            return new CommentResources(
                entity.Id,
                entity.TicketId,
                entity.Coment,
                entity.CreatedAt,
                entity.Ticket.NumeroTicket,
                entity.Ticket.Estado,
                entity.UserId, // Map the UserId
                entity.User?.Username // Map the Username
            );
        }
    }
}