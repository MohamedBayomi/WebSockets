using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebSockerServer.Middleware
{
    public static class WebSocketServerMiddlewareExtension
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
        {
            // UseMiddleware = Use (method) while method should be in your class "WebSocketServerMiddleware"
            // and should public 'Invoke' or 'InvokeAsync' 
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }
    }
}