using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Messages;

namespace Chat.Client.Infrastructure
{
    public interface ITcpChatClient
    {
        Task StartChattingAsync(CancellationTokenSource tokenSource = default);
    }

    public class TcpChatClient : ITcpChatClient
    {
        private readonly IPublisher _publisher;
        private readonly TcpSettings _tcpSettings;
        private static TcpClient _client;

        public TcpChatClient(IOptions<TcpSettings> tcpSettings, IPublisher publisher)
        {
            _publisher = publisher;
            _tcpSettings = tcpSettings.Value;
        }

        public async Task StartChattingAsync(CancellationTokenSource tokenSource = default)
        {
            if (!IPAddress.TryParse(_tcpSettings.Server, out var localAddress))
            {
                Console.WriteLine("Attention for ip address.");
                return;
            }

            _client = new TcpClient();

            Console.Write("Your first name: ");
            var nameClient = Console.ReadLine();

            await _client.ConnectAsync(localAddress, _tcpSettings.Port);
            Console.WriteLine("Client connected to server.");

            await using var messagesStream = _client.GetStream();
            await _publisher.PublishMessage(messagesStream, DateTimeOffset.UtcNow, "Hi", nameClient, true);

            var thread = new Thread(ReceiveMessage);
            thread.Start();

            try
            {
                while (tokenSource is not null && !tokenSource.IsCancellationRequested)
                {
                    var bodyMessage = Console.ReadLine();

                    await _publisher.PublishMessage(messagesStream, DateTimeOffset.UtcNow, bodyMessage, nameClient);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        static void ReceiveMessage()
        {
            try
            {
                using var messagesStream = _client.GetStream();
                using var streamParser = new TcpMessagesStreamParser(messagesStream);

                while (true)
                {
                    if (!messagesStream.Socket.Connected)
                    {
                        Console.WriteLine($"Server connection lost by endpoint {_client.Client.RemoteEndPoint}");
                        break;
                    }

                    if (streamParser.GetNextMessage() is not Message message)
                        continue;

                    Console.WriteLine($"[{message.TimeFrom}] [{message.Name}]: {message.Body}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Connection lost.");
                Console.ReadLine();
                Disconnect();
            }
        }

        private static void Disconnect()
        {
            _client?.Close();
            Environment.Exit(0);
        }
    }


}
