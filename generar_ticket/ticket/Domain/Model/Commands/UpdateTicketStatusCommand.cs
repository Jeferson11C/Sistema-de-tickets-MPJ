namespace generar_ticket.ticket.Domain.Model.Commands
{
    public record UpdateTicketStatusCommand(int TicketId, string Estado);
}