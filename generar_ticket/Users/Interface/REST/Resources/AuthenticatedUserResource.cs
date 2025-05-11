namespace generar_ticket.Users.Interface.REST.Resources
{
    public record AuthenticatedUserResource(int Id, string Token, string RefreshToken);
}