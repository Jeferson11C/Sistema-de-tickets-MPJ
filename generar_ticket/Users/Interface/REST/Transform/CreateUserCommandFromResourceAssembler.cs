using generar_ticket.Users.Domain.Model.Command;
using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Interface.REST.Transform
{
    public static partial class CreateUserCommandFromResourceAssembler
    {
        public static CreateUserCommand ToCommand(CreateUserResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));

            return new CreateUserCommand(
                resource.Id,
                resource.Dni,
                resource.Nombre,
                resource.ApePaterno,
                resource.ApeMaterno,
                resource.Username,
                resource.Password,
                resource.Rol,
                resource.Area
            );
        }
    }
}