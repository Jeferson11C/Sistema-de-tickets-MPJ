namespace generar_ticket.Users.Interface.REST.Resources
{
    public record UpdateUserResource(
        string Username,
        string Password,
        string Rol,
        string Area,
        string Estado
    )
    {
    }
}