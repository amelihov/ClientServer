using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Chat.Server.Storages;
using Common;

namespace Chat.Server.Domain
{
    public class TcpChatClient : ChatClient
    {
        private readonly TcpClient _client;
        private readonly IMessageProcessor _messageProcessor;
        private readonly IClientStorage _subscribersStorage;

        public TcpChatClient(TcpClient tcpClient, IMessageProcessor messageProcessor, IClientStorage subscribersStorage)
        {
            _client = tcpClient;
            _messageProcessor = messageProcessor;
            _subscribersStorage = subscribersStorage;
        }

        public void Chatting()
        {
            try
            {
                using var messagesStream = _client.GetStream();
                using var streamParser = new TcpMessagesStreamParser(messagesStream);

                while (true)
                {
                    if (!messagesStream.Socket.Connected)
                    {
                        Console.WriteLine($"TcpClient finished connection from address {_client.Client.RemoteEndPoint}");
                        break;
                    }

                    var message = streamParser.GetNextMessage();
                    if (message is null)
                        continue;

                    _messageProcessor.ProcessMessage(message, this).GetAwaiter().GetResult();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _subscribersStorage.RemoveClient(this.Id);
                Close();
            }
        }

        public override async Task SendMessageAsync(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            var bytes = Encoding.UTF8.GetBytes(message);

            await _client.GetStream().WriteAsync(bytes, 0, bytes.Length);
        }

        public override void Close()
        {
             _client?.Close();
        }
    }
}
