namespace generar_ticket.Observaciones.Interfaces.REST.Resources
{
    public record CreatedCommentResource(
        int TicketId,
        string Coment,
        int UserId
    );
}