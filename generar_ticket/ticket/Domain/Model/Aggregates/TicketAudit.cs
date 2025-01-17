

using System.ComponentModel.DataAnnotations.Schema;

namespace generar_ticket.ticket.Domain.Model.Aggregates;

public class TicketAudit
{
    [Column("CreatedAt")]public DateTimeOffset? CreatedDate { get; set; }
    [Column("UpdatedAt")]public DateTimeOffset? UpdatedDate { get; set; }
}