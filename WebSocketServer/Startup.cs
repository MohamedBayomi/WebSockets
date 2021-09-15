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
            services.AddWebSocketManager();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IHostApplicationLifetime hostApplicationLifetime)
        {
            app.UseWebSockets();
            // instead of calling app.Use(AcceptWebSocketAsync); we are going to call the following extension method.
            // and DI knows which method to use: public 'Invoke' or 'InvokeAsync' 
            app.UseWebSocketServer();

            app.Run(HandleRequestDelegate);
            hostApplicationLifetime.ApplicationStopping.Register(OnAppStopping);
        }

        private void OnAppStopping()
        {
            
        }

        private async Task HandleRequestDelegate(HttpContext context)
        {
            System.Console.WriteLine("--> Hello from third request delegate");
            await context.Response.WriteAsync("Hello from third request delegate");
        }

    }
}
