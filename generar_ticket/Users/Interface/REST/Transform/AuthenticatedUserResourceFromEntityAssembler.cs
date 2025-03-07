using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Interface.REST.Transform
{
    public static class AuthenticatedUserResourceFromEntityAssembler
    {
        public static AuthenticatedUserResource ToResourceFromEntity(User entity, string token, string refreshToken)
        {
            return new AuthenticatedUserResource(entity.Id, entity.Dni, entity.Password, token, refreshToken);
        }
    }
}