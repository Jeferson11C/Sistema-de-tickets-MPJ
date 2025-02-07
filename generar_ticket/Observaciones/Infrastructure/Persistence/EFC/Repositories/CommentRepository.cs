using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Observaciones.Domain.Repositories;
using generar_ticket.Observaciones.Domain.Model.Queries;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.Observaciones.Infrastructure.Persistence.EFC.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;
        private ICommentRepository _commentRepositoryImplementation;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task AddAsync(Comment entity)
        {
            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<Comment?> FindByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<Comment?> FindByGroupAsync(string group)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Coment.Contains(group));
        }

        public void Update(Comment entity)
        {
            _context.Comments.Update(entity);
            _context.SaveChanges();
        }

        public void Remove(Comment entity)
        {
            _context.Comments.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Comment>> ListAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task UpdateAsync(Comment entity)
        {
            _context.Comments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment entity)
        {
            _context.Comments.Remove(entity);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Comment> GetCommentByIdTicketAsync(int ticketId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.TicketId == ticketId);
        }

        public async Task<Comment> Handle(GetCommentByIdQuery query)
        {
            return await _context.Comments.FindAsync(query.Id);
        }
    }
}