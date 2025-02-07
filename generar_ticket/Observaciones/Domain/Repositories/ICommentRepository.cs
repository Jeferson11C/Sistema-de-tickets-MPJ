using generar_ticket.Observaciones.Domain.Model.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Shared.Domain.Repositories;
using generar_ticket.Observaciones.Domain.Model.Queries;

namespace generar_ticket.Observaciones.Domain.Repositories
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        
        Task<Comment> GetCommentByIdTicketAsync(int ticketId); 
        Task<Comment> Handle(GetCommentByIdQuery query); // Add this method
        // Add other necessary methods for the repository
    }
}