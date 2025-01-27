using generar_ticket.Users.Domain.Model.Aggregate;
using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Shared.Domain.Repositories;
using generar_ticket.Users.Domain.Model.Queries;

namespace generar_ticket.Users.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetUsersByAreaAsync(string area);
        Task<User> Handle(GetUserByIdQuery query); // Add this method
        // Add other necessary methods for the repository
        
        Task UpdateAsync(User entity);
        Task DeleteAsync(User entity);
        Task SaveChangesAsync();
    }
}