namespace generar_ticket.ticket.Domain.Model.Commands;

public record CreateTicketCommand(int TicketNumber, string Status, DateTimeOffset CreatedDate, DateTimeOffset UpdatedDate, string FirstName, string LastName);