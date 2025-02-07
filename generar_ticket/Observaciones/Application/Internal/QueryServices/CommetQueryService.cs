using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Observaciones.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using generar_ticket.Observaciones.Domain.Services;

namespace generar_ticket.Observaciones.Application.Internal.QueryServices
{
    public class CommetQueryService : ICommentQueryService
    {
        private readonly AppDbContext _context;

        public CommetQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> Handle(GetAllCommentQuery query)
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment?> Handle(GetCommentByIdQuery query)
        {
            return await _context.Comments.FindAsync(query.Id);
        }
        
        public async Task<Comment> GetCommentByIdTicketAsync(int ticketId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.TicketId == ticketId);
        }

    }
}