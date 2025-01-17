namespace generar_ticket.ticket.Domain.Model.Commands;

public record UpdateTicketCommand(int TicketId, string NewStatus);