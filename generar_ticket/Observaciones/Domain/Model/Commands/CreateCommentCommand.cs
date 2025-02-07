namespace generar_ticket.Observaciones.Domain.Model.Commands
{
    public record CreateCommentCommand(int TicketId, string Coment, int UserId);
}