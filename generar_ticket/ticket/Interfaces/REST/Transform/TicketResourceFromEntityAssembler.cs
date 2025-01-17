using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Interfaces.REST.Resources;

namespace generar_ticket.ticket.Interfaces.REST.Transform
{
    public static class TicketResourceFromEntityAssembler
    {
        public static TicketResource ToResourceFromEntity(Ticket entity)
        {
            return new TicketResource(
                entity.Id,
                entity.TicketNumber,
                entity.UserName,
                entity.Status,
                entity.CreatedDate,
                entity.UpdatedDate
            );
        }
        
    }
}