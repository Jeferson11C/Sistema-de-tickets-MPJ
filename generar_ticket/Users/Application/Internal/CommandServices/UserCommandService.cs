using System.Threading.Tasks;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Command;
using generar_ticket.Users.Domain.Services;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.Users.Application.Internal.CommandServices
{
    public class UserCommandService : IUserCommandService
    {
        private readonly AppDbContext _context;

        public UserCommandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(CreateUserCommand command)
        {
            var user = User.Create(command);
            _context.Users.Add(user); // Corrected from _context.User to _context.Users
            await _context.SaveChangesAsync();
            return user;
        }
    }
}