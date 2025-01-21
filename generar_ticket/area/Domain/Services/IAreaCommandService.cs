using generar_ticket.area.Domain.Model.Commands;
using System.Threading.Tasks;
using generar_ticket.area.Domain.Model.Aggregates;

namespace generar_ticket.area.Application.Services
{
    public interface IAreaCommandService
    {
        Task<Area?> Handle(CreateAreaCommand command);
    }
}