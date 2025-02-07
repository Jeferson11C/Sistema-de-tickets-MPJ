using System.ComponentModel.DataAnnotations.Schema;

namespace generar_ticket.Observaciones.Domain.Model.Aggregates
{
    public class CommetAudit
    {
        [Column("CreatedAt")] public DateTimeOffset? CreatedDate { get; set; }
        [Column("UpdatedAt")] public DateTimeOffset? UpdatedDate { get; set; }
    }
}