using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace generar_ticket.WebSockets;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WebSocketMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public WebSocketMiddleware(RequestDelegate next, ILogger<WebSocketMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            _logger.LogInformation("WebSocket connection established.");

            // Generar un ID único para la conexión
            var socketId = Guid.NewGuid().ToString();
            _sockets.TryAdd(socketId, webSocket);

            await HandleWebSocketConnection(context, webSocket, socketId);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket, string socketId)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = null; // Inicializar la variable

        try
        {
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation($"Received message: {message}");

                    // Procesar el mensaje recibido
                    await ProcessMessageAsync(message);
                }
            } while (!result.CloseStatus.HasValue);
        }
        catch (Exception ex)
        {
            _logger.LogError($"WebSocket error: {ex.Message}");
        }
        finally
        {
            _sockets.TryRemove(socketId, out _);

            // Verificar si result fue inicializado antes de usarlo
            if (result != null)
            {
                await webSocket.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
            }
            else
            {
                // Si result no fue inicializado, cerrar la conexión con un estado predeterminado
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }

            _logger.LogInformation("WebSocket connection closed.");
        }
    }

    private async Task ProcessMessageAsync(string message)
    {
        // Aquí puedes procesar el mensaje recibido
        // Por ejemplo, actualizar el estado de un ticket en la base de datos

        // Notificar a todos los clientes conectados
        await BroadcastMessageAsync(message);
    }

    private async Task BroadcastMessageAsync(string message)
    {
        var buffer = System.Text.Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var socket in _sockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}