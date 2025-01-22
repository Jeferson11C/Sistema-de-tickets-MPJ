using Microsoft.AspNetCore.Mvc;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Services;
using generar_ticket.ticket.Interfaces.REST.Resources;
using generar_ticket.ticket.Interfaces.REST.Transform;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;
using generar_ticket.ticket.Domain.Model.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace generar_ticket.ticket.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly PersonaService _personaService;
        private readonly AppDbContext _context;
        private readonly ITicketQueryService _ticketQueryService;

        public TicketController(PersonaService personaService, AppDbContext context, ITicketQueryService ticketQueryService)
        {
            _personaService = personaService;
            _context = context;
            _ticketQueryService = ticketQueryService;
        }

        [HttpPost]
        public async Task<ActionResult<TicketResource>> CreateTicket([FromBody] CreateTicketCommand command)
        {
            var persona = await _personaService.GetPersonaData(command.Documento);
            if (persona == null)
            {
                return NotFound(new { Message = "Persona data not found" });
            }

            var ticket = new Ticket(command, _personaService, _context);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var ticketResource = TicketResourceFromEntityAssembler.ToResourceFromEntity(ticket);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticketResource);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAllTickets()
        {
            var query = new GetAllTicketsQuery();
            var result = await _ticketQueryService.Handle(query);
    
            if (result == null || !result.Any())
            {
                Console.WriteLine("No tickets found.");
                return Ok(new List<Ticket>());
            }
    
            Console.WriteLine($"Found {result.Count()} tickets.");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            var query = new GetTicketByIdQuery(id); // Pass the id parameter to the constructor
            var result = await _ticketQueryService.Handle(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("area/{areaNombre}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByArea(string areaNombre)
        {
            var query = new GetTicketsByAreaQuery(areaNombre); // Pass the areaNombre parameter to the constructor
            var result = await _ticketQueryService.Handle(query);
            return Ok(result);
        }
    }
}