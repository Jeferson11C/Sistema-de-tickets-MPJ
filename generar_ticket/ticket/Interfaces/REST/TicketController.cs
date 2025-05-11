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
using Microsoft.AspNetCore.Authorization;

namespace generar_ticket.ticket.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly PersonaService _personaService;
        private readonly AppDbContext _context;
        private readonly ITicketQueryService _ticketQueryService;
        private readonly PrintService _printService;

        public TicketController(PersonaService personaService, AppDbContext context, ITicketQueryService ticketQueryService, PrintService printService)
        {
            _personaService = personaService;
            _context = context;
            _ticketQueryService = ticketQueryService;
            _printService = printService;
        }

        [HttpPost]
        public async Task<ActionResult<TicketResource>> CreateTicket([FromBody] CreateTicketCommand command)
        {
            try
            {
                var persona = await _personaService.GetPersonaData(command.Documento);
                if (persona == null)
                {
                    return NotFound(new { Message = "Persona data not found" });
                }

                if (await _personaService.EsMenorDeEdad(command.Documento))
                {
                    return BadRequest(new { Message = "DNI corresponde a un menor de edad" });
                }

                var ticket = new Ticket(command, _personaService, _context);
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                var ticketResource = TicketResourceFromEntityAssembler.ToResourceFromEntity(ticket);

                // Print the ticket automatically
                await _printService.PrintTicketAsync(ticketResource);

                return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticketResource);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while creating the ticket: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, "An internal server error occurred.");
            }
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

       
        [Authorize]
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

        [Authorize]
        [HttpGet("area/{areaNombre}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByArea(string areaNombre)
        {
            var query = new GetTicketsByAreaQuery(areaNombre); // Pass the areaNombre parameter to the constructor
            var result = await _ticketQueryService.Handle(query);
            return Ok(result);
        }
        
        
        [Authorize]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusCommand command)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            ticket.UpdateTicketStatus(command);
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [Authorize]
        [HttpPut("{id}/area")]
        public async Task<IActionResult> UpdateTicketArea(int id, [FromBody] UpdateTicketAreaCommand command)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            ticket.UpdateTicketArea(command, _context);

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [Authorize]
        [HttpPost("installed")]
        public ActionResult<List<PrinterInfo>> GetInstalledPrinters()
        {
            var printers = PrinterService.GetInstalledPrinters();
            return Ok(printers);
        }
        
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByStatus(string status)
        {
            var query = new GetTicketsByStatusQuery(status); // Pass the status parameter to the constructor
            var result = await _ticketQueryService.Handle(query);

            if (result == null || !result.Any())
            {
                return NotFound(new { Message = "No tickets found with the specified status." });
            }

            return Ok(result);
        }
        
    }
}