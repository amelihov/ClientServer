using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;

namespace Chat.Server.Domain
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ();

        public WebSocketConnectionManager()
        {
        }

        public void AddSocket(string id, WebSocket socket)
        {
            _sockets.TryAdd(id, socket);
        }
        public void RemoveSocket(string id)
        {
            _sockets.TryRemove(id, out _);
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
    }
}
