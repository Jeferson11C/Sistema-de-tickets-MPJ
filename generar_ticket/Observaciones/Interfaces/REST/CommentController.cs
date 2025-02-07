using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.Observaciones.Domain.Model.Commands;
using generar_ticket.Observaciones.Domain.Services;
using generar_ticket.Observaciones.Interfaces.REST.Resources;
using generar_ticket.Observaciones.Interfaces.REST.Transform;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.ticket.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace generar_ticket.Observaciones.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentCommandService _commentCommandService;
        private readonly CommentResourceFromEntityAssembler _commentResourceFromEntityAssembler;
        private readonly AppDbContext _context;

        public CommentController(ICommentCommandService commentCommandService, CommentResourceFromEntityAssembler commentResourceFromEntityAssembler, AppDbContext context)
        {
            _commentCommandService = commentCommandService;
            _commentResourceFromEntityAssembler = commentResourceFromEntityAssembler;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CommentResources>> AddComment([FromBody] CreatedCommentResource resource)
        {
            var command = new CreateCommentCommand(resource.TicketId, resource.Coment, resource.UserId); // Include UserId
            var (commentId, numeroTicket) = await _commentCommandService.AddCommentToTicketAsync(command.TicketId, command.Coment, command.UserId);

            var comment = new Comment(command.TicketId, command.Coment)
            {
                Id = commentId,
                CreatedAt = DateTime.UtcNow.ToLocalTime(),
                Ticket = new Ticket { NumeroTicket = numeroTicket },
                UserId = command.UserId // Set UserId
            };
            var createdResource = _commentResourceFromEntityAssembler.ToResource(comment);

            return CreatedAtAction(nameof(AddComment), new { id = createdResource.Id }, createdResource);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentResources>> GetCommentById(int id)
        {
            var comment = await _context.Comments.Include(c => c.Ticket).Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.CreatedAt = comment.CreatedAt.ToLocalTime(); // Convert to local time
            var resource = _commentResourceFromEntityAssembler.ToResource(comment);
            return Ok(resource);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentResources>>> GetAllComments()
        {
            var comments = await _context.Comments.Include(c => c.Ticket).Include(c => c.User).ToListAsync();
            foreach (var comment in comments)
            {
                comment.CreatedAt = comment.CreatedAt.ToLocalTime(); // Convert to local time
            }
            var resources = comments.Select(c => _commentResourceFromEntityAssembler.ToResource(c)).ToList();
            return Ok(resources);
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<ActionResult<IEnumerable<CommentResources>>> GetCommentsByTicketId(int ticketId)
        {
            var comments = await _context.Comments.Include(c => c.Ticket).Include(c => c.User).Where(c => c.TicketId == ticketId).ToListAsync();
            if (comments == null || !comments.Any())
            {
                return Ok(new { message = "no atendido" });
            }

            foreach (var comment in comments)
            {
                comment.CreatedAt = comment.CreatedAt.ToLocalTime(); // Convert to local time
            }

            var resources = comments.Select(c => _commentResourceFromEntityAssembler.ToResource(c)).ToList();
            return Ok(resources);
        }
        
    }
}