using generar_ticket.area.Domain.Model.Aggregates;
using generar_ticket.area.Interfaces.REST.Resources;

namespace generar_ticket.area.Interfaces.REST.Transform
{
    public class CreateAreaCommandFromResourcesAssembler
    {
        public Area ToEntity(CreatedAreaResources resource)
        {
            return new Area
            {
                Nombre = resource.Nombre,
                Codigo = resource.Codigo
            };
        }
    }
}