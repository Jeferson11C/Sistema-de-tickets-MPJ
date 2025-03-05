
// Ticket.cs
using System;
using System.Globalization;
using System.Linq;
using generar_ticket.Observaciones.Domain.Model.Aggregates;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Services;
using generar_ticket.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace generar_ticket.ticket.Domain.Model.Aggregates
{
    public class Ticket
    {
        public int Id { get; set; }
        public string NumeroTicket { get; set; }
        public string AreaNombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Documento { get; set; }
        public string Nombres { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Estado { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        // Parameterless constructor for EF Core
        public Ticket() { }

        // Constructor with parameters
        public Ticket(CreateTicketCommand command, PersonaService personaService, AppDbContext context)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (personaService == null)
                throw new ArgumentNullException(nameof(personaService));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Documento = command.Documento;
            AreaNombre = command.AreaNombre;
            Fecha = DateTime.Now;
            Estado = "En espera";
            NumeroTicket = GenerateTicketNumber(AreaNombre, context);

            var persona = personaService.GetPersonaData(command.Documento).Result;
            if (persona == null)
                throw new Exception("Persona data not found");

            Nombres = persona.Nombres;
            ApePaterno = persona.ApePaterno;
            ApeMaterno = persona.ApeMaterno;
        }

        private string GenerateTicketNumber(string areaNombre, AppDbContext context)
        {
            var today = DateTime.Today;
            var lastTicket = context.Tickets
                .Where(t => t.AreaNombre == areaNombre && t.Fecha.Date == today)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();

            int nextCounter = lastTicket != null ? int.Parse(lastTicket.NumeroTicket.Split('-')[1]) + 1 : 1;

            string newTicketNumber = $"{areaNombre.Substring(0, 1).ToUpper()}-{nextCounter:D3}";

            return newTicketNumber;
        }

        public string GetFormattedFecha()
        {
            return Fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        
        public void UpdateTicketStatus(UpdateTicketStatusCommand command)
        {
            Estado = command.Estado;
            if (Estado == "Resuelto" || Estado == "Cancelado")
            {
                UpdatedAt = DateTime.Now; // Automatically set the current date and time
            }
        }
        
        public void UpdateTicketArea(UpdateTicketAreaCommand command, AppDbContext context)
        {
            AreaNombre = command.NewArea;
            NumeroTicket = GenerateTicketNumber(command.NewArea, context);
        }
    }
}