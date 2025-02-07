namespace generar_ticket.Observaciones.Interfaces.REST.Resources
{
    public record CommentResources(
        int Id,
        int TicketId,
        string Coment,
        DateTime CreatedAt,
        string NumeroTicket,
        string Estado,
        int UserId, // New property
        string Username // New property
    );
}