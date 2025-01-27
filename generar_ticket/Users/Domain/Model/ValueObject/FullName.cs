namespace generar_ticket.Users.Domain.Model.ValueObject
{
    public record FullName(string Nombre, string ApePaterno, string ApeMaterno)
    {
        public FullName() : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public FullName(string nombre) : this(nombre, string.Empty, string.Empty)
        {
        }

        public string NombreCompleto => $"{Nombre} {ApePaterno} {ApeMaterno}";
    }
}