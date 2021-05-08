using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
    public class WebSocketChatClient : ChatClient
    {
        private readonly WebSocket _socket;

        public WebSocketChatClient(WebSocket socket)
        {
            _socket = socket;
        }
        public override async Task SendMessageAsync(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (_socket.State != WebSocketState.Open)
                return;

            await _socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.UTF8.GetBytes(message),
                    offset: 0,
                    count: message.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }

        public override void Close()
        {
            try
            {
                _socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                    statusDescription: "Closed by the ConnectionManager",
                    cancellationToken: CancellationToken.None).GetAwaiter().GetResult();
            }
            finally
            {
                _socket?.Dispose();
            }
        }
    }
}
