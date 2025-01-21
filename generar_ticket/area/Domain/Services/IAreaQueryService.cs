using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Domain.Model.Queries;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace generar_ticket.area.Domain.Services
{
    public interface IAreaQueryService
    {
        Task<IEnumerable<Area?>> Handle(GetAllAreasQuery query);
        Task<Area?> Handle(GetAreaByIdQuery query);
    }
}