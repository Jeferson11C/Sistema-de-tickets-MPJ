using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Interfaces.REST.Resources;

namespace generar_ticket.area.Interfaces.REST.Transform
{
    public class AreaResourcesFromEntityAssembler
    {
        public AreaResources ToResource(Area area)
        {
            return new AreaResources(area.Id, area.Nombre, area.Codigo);
        }
    }
}