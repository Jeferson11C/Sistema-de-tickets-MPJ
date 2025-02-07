using generar_ticket.Users.Domain.Model;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Interface.REST.Transform
{
    public static class UserResourceFromEntityAssembler
    {
        public static UserResource ToResourceFromEntity(User entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new UserResource(
                entity.Id,
                entity.Dni,
                entity.NombreCompletoDisplay, // Use the display property for full name
                entity.Username,
                entity.Password,
                entity.Rol,
                entity.Area,
                entity.Estado
            );
        }
    }
}