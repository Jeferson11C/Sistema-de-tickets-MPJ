using generar_ticket.Users.Domain.Model.Command;

using generar_ticket.Users.Interface.REST.Resources;

namespace generar_ticket.Users.Interface.REST.Transform
{
    public static class SignInCommandFromResourceAssembler
    {
        public static SignInCommand ToCommandFromResource(SignInResource resource)
        {
            return new SignInCommand(resource.Username, resource.Password);
        }
    }
}