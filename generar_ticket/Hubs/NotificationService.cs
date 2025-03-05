using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyEntityUpdated(string entityName, object entity)
    {
        await _hubContext.Clients.All.SendAsync("EntityUpdated", entityName, entity);
    }
}