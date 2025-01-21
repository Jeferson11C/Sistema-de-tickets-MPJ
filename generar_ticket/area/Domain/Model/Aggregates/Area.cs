using generar_ticket.area.Domain.Model.Commands;

namespace generar_ticket.area.Domain.Model.Aggregates
{
    public class Area
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }

        public Area()
        {
        }

        public static Area? Create(CreateAreaCommand command)
        {
            return new Area
            {
                Nombre = command.Nombre,
                Codigo = command.Codigo,
            };
        }
    }
}
