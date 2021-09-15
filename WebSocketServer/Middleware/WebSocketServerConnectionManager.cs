using System.Collections.Concurrent;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSockerServer.Models;
using System.Text.Json;
using System.Linq;

namespace WebSockerServer.Middleware
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task AddSocket(WebSocket socket)
        {
            var ConnID = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=');
            _sockets.TryAdd(ConnID, socket);
            System.Console.WriteLine($"--> Connection Added: {ConnID}");
            await SendAsync(socket, new Payload { From = "Server", To = "Client", Message = "ConnID: " + ConnID });
        }
        public void RemoveSocket(WebSocket socket)
        {
            var item = _sockets.FirstOrDefault(i => i.Value == socket);
            _sockets.TryRemove(item);
            System.Console.WriteLine($"--> Connection removed: {item.Key}");
        }

        public async Task RouteMessage(string message)
        {
            Payload payload = JsonSerializer.Deserialize<Payload>(message);

            if (_sockets.TryGetValue(payload.To, out WebSocket socket))
            {
                await SendAsync(socket, payload);
            }
            else
            {
                payload.To = "*";
                await BroadCast(payload);
            }
        }

        private async Task BroadCast(Payload payload)
        {
            Parallel.ForEach(_sockets, (socket) =>
            {
                SendAsync(socket.Value, payload);
            });
        }

        private async Task SendAsync(WebSocket socket, Payload payload)
        {
            if (socket.State == WebSocketState.Open)
            {
                var message = JsonSerializer.Serialize(payload);
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}