using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Chat.Server.Storages;
using Common.Messages;
using Common.Messages.Enums;
using Newtonsoft.Json;

namespace Chat.Server.Domain
{
    public class WebSocketMessageHandler : WebSocketHandler
    {
        public WebSocketMessageHandler(WebSocketConnectionManager webSocketConnectionManager, IClientStorage subscribersStorage, IMessageStorage messageStorage) :
            base(webSocketConnectionManager, subscribersStorage, messageStorage)
        {
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            if (result != null)
            {
                var message = $"{Encoding.UTF8.GetString(buffer, 0, result.Count)}";

                var objectMessage = JsonConvert.DeserializeObject<Message>(message);
                if (objectMessage.Type == MessageType.Subscribe)
                    await SendSubscribeMessageAsync(socket, objectMessage.Name);

                await SendMessageToAllAsync(message);
            }
        }
    }
}
