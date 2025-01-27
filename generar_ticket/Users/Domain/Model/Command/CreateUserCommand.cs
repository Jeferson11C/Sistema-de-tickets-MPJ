namespace generar_ticket.Users.Domain.Model.Command
{
    public record CreateUserCommand(
        int Id,
        string Nombre,
        string ApePaterno,
        string ApeMaterno,
        string Username,
        string Password,
        string Rol,
        string Area);
}