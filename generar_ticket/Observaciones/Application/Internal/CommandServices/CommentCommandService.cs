using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.Observaciones.Domain.Services
{
    public class CommentCommandService : ICommentCommandService
    {
        private readonly AppDbContext _context;

        public CommentCommandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(int commentId, string numeroTicket)> AddCommentToTicketAsync(int ticketId, string comment, int userId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Ticket not found");
            }

            var newComment = new Comment(ticketId, comment)
            {
                UserId = userId, // Set UserId
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            return (newComment.Id, ticket.NumeroTicket);
        }
    }
}