namespace generar_ticket.Users.Interface.REST.Resources
{
    public record UpdateUserResource(
        int Id,
        string Nombre,
        string ApePaterno,
        string ApeMaterno,
        string Username,
        string Password,
        string Rol,
        string Area
    );
}