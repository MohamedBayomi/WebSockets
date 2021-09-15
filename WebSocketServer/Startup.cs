using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using WebSockerServer.Middleware;

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
            // instead of calling app.Use(AcceptWebSocketAsync); we are going to call the following extension method.
            // and DI knows which method to use: public 'Invoke' or 'InvokeAsync' 
            app.UseWebSocketServer();

            app.Run(HandleRequestDelegate);
        }

        private async Task HandleRequestDelegate(HttpContext context)
        {
            System.Console.WriteLine("--> Hello from third request delegate");
            await context.Response.WriteAsync("Hello from third request delegate");
        }

    }
}
