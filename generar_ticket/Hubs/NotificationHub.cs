using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task NotifyEntityCreated(string entityName, object entity)
    {
        await Clients.All.SendAsync("EntityCreated", entityName, entity);
    }

    public async Task NotifyEntityUpdated(string entityName, object entity)
    {
        await Clients.All.SendAsync("EntityUpdated", entityName, entity);
    }

    public async Task NotifyEntityDeleted(string entityName, object entity)
    {
        await Clients.All.SendAsync("EntityDeleted", entityName, entity);
    }
}
