using generar_ticket.area.Domain.Model.Aggregates;

namespace generar_ticket.area.Domain.Repositories
{
    public interface IAreaRepository
    {
        IEnumerable<Area?> GetAllAreas();
        Area? GetAreaById(int id);
        void AddArea(Area? area);
    }
}