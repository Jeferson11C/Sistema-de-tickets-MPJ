namespace generar_ticket.Shared.Domain.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}