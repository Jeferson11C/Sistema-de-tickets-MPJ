using generar_ticket.ticket.Domain.Model.ValueObjects;

namespace generar_ticket.ticket.Interfaces.REST.Resources;

public record CreatedTicketResource(int Id, int TicketNumber, UserName UserName, string Status, DateTimeOffset CreatedDate, DateTimeOffset UpdatedDate);