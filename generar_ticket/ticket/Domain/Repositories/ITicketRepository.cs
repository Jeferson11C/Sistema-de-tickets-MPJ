using generar_ticket.ticket.Domain.Model.Aggregates;
using System.Collections.Generic;
using System.Threading.Tasks;
using generar_ticket.Shared.Domain.Repositories;

namespace generar_ticket.ticket.Domain.Repositories
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(int id);
        Task<Ticket> GetTicketByNumberAsync(string numeroTicket);
        // Add other necessary methods for the repository
    }
}