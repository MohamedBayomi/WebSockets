using System.Threading;
using System.Security.AccessControl;
using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;

namespace WebSocketServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();
            app.Use(AcceptWebSocketAsync);
            app.Run(HandleRequestDelegate);
        }

        private async Task AcceptWebSocketAsync(HttpContext context, Func<Task> next)
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
                await next();
            }
        }

        private void PrintMessage(WebSocketReceiveResult result, byte[] buffer)
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                System.Console.WriteLine("--> Message Received");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                System.Console.WriteLine("--> Close Message Received");
            }
        }

        private async Task HandleRequestDelegate(HttpContext context)
        {
            System.Console.WriteLine("--> Hello from third request delegate");
            await context.Response.WriteAsync("Hello from third request delegate");
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
