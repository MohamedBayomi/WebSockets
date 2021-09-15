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
        public WebSocketServerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            WriteRequestParam(context);
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                System.Console.WriteLine("--> Web Socket Connected...");

                await ReceiveMessage(socket, PrintMessage);
            }
            else
            {
                System.Console.WriteLine("--> Hello from second request delegate");
                await _next(context);
            }
        }
        
        private void PrintMessage(WebSocketReceiveResult result, byte[] buffer)
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                System.Console.WriteLine($"--> Message Received: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                System.Console.WriteLine("--> Close Message Received");
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

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024*4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

    }
}