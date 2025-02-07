namespace generar_ticket.Users.Interface.REST.Resources
{
    public record AuthenticatedUserResource(int Id, string Username,string Password, string Token);
}