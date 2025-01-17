using generar_ticket.Shared.Domain.Repositories;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Queries;
using System.Threading.Tasks;

namespace generar_ticket.ticket.Domain.Repositories
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<Ticket?> GetTicketByIdAsync(GetTicketByIdQuery query);
        Task<Ticket?> GetTicketByNumberAsync(GetTicketByNumberQuery query);
        Task UpdateAsync(Ticket ticket); // Ensure this method exists here
    }
}