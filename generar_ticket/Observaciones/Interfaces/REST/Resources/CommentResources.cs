namespace generar_ticket.Observaciones.Interfaces.REST.Resources
{
    public record CommentResources(
        int Id,
        int TicketId,
        string Coment,
        DateTime CreatedAt,
        string NumeroTicket,
        string Estado,
        int UserId,
        string NombreCompleto // Changed to string
    );
}