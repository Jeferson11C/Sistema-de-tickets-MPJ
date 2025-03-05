namespace generar_ticket.Users.Interface.REST.Resources;

public record UserResource(
    int Id,
    string Dni,
    string NombreCompleto,
    string Ventanilla,
    string Password,
    string Rol,
    string Area,
    string Estado
);