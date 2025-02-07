namespace generar_ticket.Users.Interface.REST.Resources;

public record CreateUserResource(
    int Id,
    string Dni,
    string Nombre,
    string ApePaterno,
    string ApeMaterno,
    string Username,
    string Password,
    string Rol,
    string Area
);