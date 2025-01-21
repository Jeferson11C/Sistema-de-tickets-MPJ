namespace generar_ticket.ticket.Interfaces.REST.Resources
{
    public record TicketResource(
        int Id,
        string NumeroTicket,
        string AreaNombre,
        DateTime Fecha,
        string Documento,
        string Nombres,
        string ApePaterno,
        string ApeMaterno,
        string Estado
    );
}