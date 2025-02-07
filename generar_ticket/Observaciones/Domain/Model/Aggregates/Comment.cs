

using generar_ticket.Observaciones.Domain.Model.Commands;
using generar_ticket.ticket.Domain.Model.Aggregates;
using generar_ticket.Users.Domain.Model.Aggregate;

namespace generar_ticket.Observaciones.Domain.Model.Aggregates
{
    public class Comment
    {
        public int Id { get; set; }
        public int TicketId { get; private set; }
        public string Coment { get; private set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public Ticket Ticket { get; set; }
        public User User { get; set; } // Navigation property

        // Constructor with parameters for creating a Comment
        public Comment(int ticketId, string coment)
        {
            TicketId = ticketId;
            Coment = coment ?? throw new ArgumentNullException(nameof(coment));
            CreatedAt = DateTime.UtcNow;
        }

        // Method to create a Comment from a CreateCommentCommand
        public static Comment CreateFromCommand(CreateCommentCommand command)
        {
            return new Comment(command.TicketId, command.Coment)
            {
                UserId = command.UserId // Set UserId
            };
        }

        // Optionally, you can add methods to update the comment content
        public void UpdateContent(string newComent)
        {
            Coment = newComent ?? throw new ArgumentNullException(nameof(newComent));
        }
    }
}