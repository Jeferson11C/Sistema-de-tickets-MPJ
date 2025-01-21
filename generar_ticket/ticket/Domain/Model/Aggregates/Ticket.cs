using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using generar_ticket.ticket.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Services;

namespace generar_ticket.ticket.Domain.Model.Aggregates
{
    public class Ticket
    {
        private static int _lastId = 0;

        public int Id { get; private set; }
        public string NumeroTicket { get; set; }
        [NotMapped]
        public string AreaNombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Documento { get; set; }
        public string Nombres { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Estado { get; set; }

        private static Dictionary<string, int> areaTicketCounters = new Dictionary<string, int>();

        // Parameterless constructor for EF Core
        public Ticket() { }

        // Constructor with parameters
        public Ticket(CreateTicketCommand command, PersonaService personaService)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            if (personaService == null)
                throw new ArgumentNullException(nameof(personaService));

            Id = ++_lastId;
            Documento = command.Documento;
            AreaNombre = command.AreaNombre;
            Fecha = DateTime.Now;
            Estado = "En espera";
            NumeroTicket = GenerateTicketNumber(AreaNombre);

            var persona = personaService.GetPersonaData(command.Documento).Result;
            if (persona == null)
                throw new Exception("Persona data not found");

            Nombres = persona.Nombres;
            ApePaterno = persona.ApePaterno;
            ApeMaterno = persona.ApeMaterno;
        }

        private string GenerateTicketNumber(string areaNombre)
        {
            string areaCode = areaNombre.Substring(0, 1).ToUpper();

            if (!areaTicketCounters.ContainsKey(areaCode))
            {
                areaTicketCounters[areaCode] = 0;
            }

            areaTicketCounters[areaCode]++;
            return $"{areaCode}-{areaTicketCounters[areaCode]:D3}";
        }

        public string GetFormattedFecha()
        {
            return Fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}