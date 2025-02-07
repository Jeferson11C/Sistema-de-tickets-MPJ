using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Observaciones.Domain.Model.Queries;

namespace generar_ticket.Observaciones.Domain.Services
{
    public interface ICommentQueryService
    {
        Task<IEnumerable<Comment>> Handle(GetAllCommentQuery query);
        Task<Comment?> Handle(GetCommentByIdQuery query);
    }
}