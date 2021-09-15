using System.Net.Http;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebSockerServer.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketServerConnectionManager _manager;
        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //WriteRequestParam(context);
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                System.Console.WriteLine("--> Web Socket Connected...");
                await _manager.AddSocket(socket);

                await ReceiveMessage(socket);
            }
            else
            {
                System.Console.WriteLine("--> Hello from second request delegate");
                await _next(context);
            }
        }

        public void WriteRequestParam(HttpContext context)
        {
            System.Console.WriteLine("Request Method: " + context.Request.Method);
            System.Console.WriteLine("Request Protocol: " + context.Request.Protocol);

            if (context.Request.Headers != null)
            {
                foreach (var h in context.Request.Headers)
                {
                    System.Console.WriteLine("--> " + h.Key + ": " + h.Value);
                }
            }
        }

        private async Task ReceiveMessage(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    System.Console.WriteLine($"--> Message Received: {message}");
                    await _manager.RouteMessage(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    System.Console.WriteLine("--> Close Message Received");
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    _manager.RemoveSocket(socket);
                }
            }
        }

    }
}