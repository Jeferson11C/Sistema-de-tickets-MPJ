using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Domain.Model.Queries;
using generar_ticket.area.Domain.Repositories;
using generar_ticket.area.Domain.Services;

namespace generar_ticket.area.Application.Internal.QueryService
{
    public class AreaQueryService : IAreaQueryService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaQueryService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public async Task<IEnumerable<Area?>> Handle(GetAllAreasQuery query)
        {
            return await Task.FromResult(_areaRepository.GetAllAreas());
        }

        public async Task<Area?> Handle(GetAreaByIdQuery query)
        {
            return await Task.FromResult(_areaRepository.GetAreaById(query.Id));
        }
    }
}