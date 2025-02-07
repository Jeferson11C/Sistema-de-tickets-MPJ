using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Command;

namespace generar_ticket.Users.Domain.Services
{
    public interface IUserCommandService
    {
        Task<User> Handle(CreateUserCommand command);
        Task<(User user, string token)> Handle(SignInCommand command);
    }
}