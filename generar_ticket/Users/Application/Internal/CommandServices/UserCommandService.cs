using System.Threading.Tasks;
using generar_ticket.Users.Domain.Model.Aggregate;
using generar_ticket.Users.Domain.Model.Command;
using generar_ticket.Users.Domain.Services;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

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
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<(User user, string token)> Handle(SignInCommand command)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Dni == command.Dni && u.Password == command.Password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid DNI or password.");
            }

            // Generate a token (this is just a placeholder, implement your token generation logic)
            var token = "generated_token";

            return (user, token);
        }
    }
}