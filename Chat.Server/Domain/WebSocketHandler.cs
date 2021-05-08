using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Chat.Server.Storages;
using Common.Messages;

namespace Chat.Server.Domain
{
    public abstract class WebSocketHandler
    {
        private readonly IClientStorage _subscribersStorage;
        private readonly IMessageStorage _messageStorage;
        protected WebSocketConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager, IClientStorage subscribersStorage,
            IMessageStorage messageStorage)
        {
            _subscribersStorage = subscribersStorage;
            _messageStorage = messageStorage;
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            await Task.Run(() =>
            {
                var client = new WebSocketChatClient(socket);
                _subscribersStorage.AddOrUpdateClient(client, client.Id);
                WebSocketConnectionManager.AddSocket(client.Id, socket);
            });
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await Task.Run(() =>
            {
                var id = WebSocketConnectionManager.GetId(socket);
                _subscribersStorage.RemoveClient(id);
                WebSocketConnectionManager.RemoveSocket(id);
            });
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in _subscribersStorage.GetAll())
            {
                await pair.Key.SendMessageAsync(message);
            }
        }
        public async Task SendSubscribeMessageAsync(WebSocket socket, string name)
        {
            var id = WebSocketConnectionManager.GetId(socket);
            var client = _subscribersStorage.GetClientById(id);
            client.Info = "Websocket";
            client.Name = string.IsNullOrEmpty(name) ? "Incognito" : name;
            _subscribersStorage.AddOrUpdateClient(client, client.Id);

            foreach (var m in _messageStorage.GetLastThirtyMessages())
            {
                await client.SendMessageAsync(m);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}
