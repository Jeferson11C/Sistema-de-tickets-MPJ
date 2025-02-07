using System.Threading.Tasks;

namespace generar_ticket.Observaciones.Domain.Services
{
    public interface ICommentCommandService
    {
        Task<(int commentId, string numeroTicket)> AddCommentToTicketAsync(int ticketId, string comment, int userId); // Include UserId
    }
}