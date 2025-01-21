using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Domain.Model.Commands;
using generar_ticket.area.Domain.Repositories;
using System.Threading.Tasks;
using generar_ticket.area.Application.Services;

namespace generar_ticket.area.Application.Internal.CommandServices
{
    public class AreaCommandService : IAreaCommandService
    {
        private readonly IAreaRepository _areaRepository;

        public AreaCommandService(IAreaRepository areaRepository)
        {
            _areaRepository = areaRepository;
        }

        public async Task<Area?> Handle(CreateAreaCommand command)
        {
            var area = Area.Create(command);
            _areaRepository.AddArea(area);
            return await Task.FromResult(area);
        }
    }
}