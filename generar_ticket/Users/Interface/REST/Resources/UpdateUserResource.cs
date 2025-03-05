namespace generar_ticket.Users.Interface.REST.Resources
{
    public record UpdateUserResource(
        string Ventanilla,
        string Password,
        string Rol,
        string Area,
        string Estado
    )
    {
    }
}