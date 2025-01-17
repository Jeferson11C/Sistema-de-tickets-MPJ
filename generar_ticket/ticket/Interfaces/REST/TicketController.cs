using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.ticket.Interfaces.REST.Resources;
using generar_ticket.ticket.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace generar_ticket.ticket.Interfaces.REST
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateTicket([FromBody] CreatedTicketResource createdTicketResource)
        {
            var createTicketCommand = CreateTicketCommandFromResourceAssembler.ToCommandFromResource(createdTicketResource);
            // Logic to handle the creation of the ticket using createTicketCommand
            // For example, calling a service to handle the command
            return CreatedAtAction(nameof(GetTicketById), new { id = createTicketCommand.TicketNumber }, createTicketCommand);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTicket(int id, [FromBody] UpdatedTicketResource updatedTicketResource)
        {
            if (id != updatedTicketResource.Id)
            {
                return BadRequest();
            }

            var updateTicketCommand = UpdateTicketCommandFromResourceAssembler.ToCommandFromResource(updatedTicketResource);
            // Logic to handle the update of the ticket using updateTicketCommand
            // For example, calling a service to handle the command
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetTicketById(int id)
        {
            // Logic to retrieve the ticket by id
            // For example, calling a service to get the ticket
            Ticket ticket = null; // Replace with actual retrieval logic
            if (ticket == null)
            {
                return NotFound();
            }

            var ticketResource = TicketResourceFromEntityAssembler.ToResourceFromEntity(ticket);
            return Ok(ticketResource);
        }
    }
}