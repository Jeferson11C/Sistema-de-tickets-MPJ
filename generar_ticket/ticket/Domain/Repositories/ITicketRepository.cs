using generar_ticket.ticket.Domain.Model.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Shared.Domain.Repositories;
using generar_ticket.ticket.Domain.Model.Queries;

namespace generar_ticket.ticket.Domain.Repositories
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(int id);
        Task<Ticket> Handle(GetTicketByIdQuery query); // Add this method
        // Add other necessary methods for the repository
    }
}