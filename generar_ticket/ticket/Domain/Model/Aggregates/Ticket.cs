using System.ComponentModel.DataAnnotations.Schema;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Model.ValueObjects;
using System.Globalization;

namespace generar_ticket.ticket.Domain.Model.Aggregates;

public class Ticket
{
    private static int _lastTicketNumber = 0;
    private static int _lastId = 0;

    public int Id { get; private set; }
    public int TicketNumber { get; private set; }

    // Propiedades descompuestas para el mapeo con EF Core
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    // Propiedad para trabajar con el value object en el dominio
    [NotMapped]
    public UserName UserName
    {
        get => new UserName(FirstName, LastName);
        private set
        {
            if (value == null) throw new ArgumentNullException(nameof(UserName));
            FirstName = value.FirstName;
            LastName = value.LastName;
        }
    }

    public string Status { get; private set; }
    public DateTimeOffset CreatedDate { get; private set; }
    public DateTimeOffset UpdatedDate { get; private set; }

    // Constructor privado para EF Core
    private Ticket() { }

    public Ticket(UserName userName, string status, DateTimeOffset createdDate, DateTimeOffset updatedDate)
    {
        Id = ++_lastId;
        TicketNumber = ++_lastTicketNumber;
        UserName = userName;
        Status = status;
        CreatedDate = createdDate;
        UpdatedDate = updatedDate;
    }

    public void UpdateStatus(UpdateTicketCommand command)
    {
        var validStatuses = new[] { "En espera", "En atención", "Finalizado", "Cancelado" };
        if (validStatuses.Contains(command.NewStatus))
        {
            Status = command.NewStatus;
            UpdatedDate = DateTimeOffset.UtcNow;
        }
        else
        {
            throw new ArgumentException("Invalid status");
        }
    }

    public static Ticket Create(CreateTicketCommand command, UserName userName)
    {
        return new Ticket(userName, command.Status, command.CreatedDate, command.UpdatedDate);
    }

    public string FormattedCreatedDate => CreatedDate.ToString("dd/MM/yy HH:mm", CultureInfo.InvariantCulture);
    public string FormattedUpdatedDate => UpdatedDate.ToString("dd/MM/yy HH:mm", CultureInfo.InvariantCulture);
}
