using Microsoft.AspNetCore.Mvc;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Services;
using generar_ticket.ticket.Interfaces.REST.Resources;
using generar_ticket.ticket.Interfaces.REST.Transform;

namespace generar_ticket.ticket.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly PersonaService _personaService;

        public TicketController(PersonaService personaService)
        {
            _personaService = personaService;
        }

        [HttpPost]
        public ActionResult<TicketResource> CreateTicket([FromBody] CreateTicketCommand command)
        {
            var persona = _personaService.GetPersonaData(command.Documento).Result;
            if (persona == null)
            {
                return NotFound(new { Message = "Persona data not found" });
            }

            var ticket = new Ticket(command, _personaService);
            var ticketResource = TicketResourceFromEntityAssembler.ToResourceFromEntity(ticket);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id }, ticketResource);
        }

        [HttpGet("{id}")]
        public ActionResult<TicketResource> GetTicketById(int id)
        {
            // Simulate getting a ticket by ID
            var command = new CreateTicketCommand("12345678", "Example Area");
            var ticket = new Ticket(command, _personaService);

            var ticketResource = TicketResourceFromEntityAssembler.ToResourceFromEntity(ticket);
            return Ok(ticketResource);
        }
    }
}