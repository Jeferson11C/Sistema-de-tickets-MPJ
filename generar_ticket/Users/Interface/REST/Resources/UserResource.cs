namespace generar_ticket.Users.Interface.REST.Resources;

public record UserResource(
    int Id,
    string NombreCompleto,
    string Username,
    string Password,
    string Rol,
    string Area
);