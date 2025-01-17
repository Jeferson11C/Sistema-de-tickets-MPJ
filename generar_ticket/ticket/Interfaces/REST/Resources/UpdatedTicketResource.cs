namespace generar_ticket.ticket.Interfaces.REST.Resources;

public record UpdatedTicketResource(int Id, string Status, DateTimeOffset UpdatedDate);