namespace generar_ticket.Users.Domain.Model.Command
{
    public record UpdateUserCommand(
        int Id,
        string Username,
        string Password,
        string Rol,
        string Area
    );
}