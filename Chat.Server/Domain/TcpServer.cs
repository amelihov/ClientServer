using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Chat.Server.Storages;
using Common;
using Microsoft.Extensions.Options;

namespace Chat.Server.Domain
{
    public interface ITcpServer
    {
        void StartListening(CancellationTokenSource cts = default);
        void Disconnect();
    }

    public class TcpServer : ITcpServer
    {
        private TcpListener _server;

        private readonly IClientStorage _subscribersStorage;
        private readonly IMessageProcessor _messageProcessor;
        private readonly TcpSettings _tcpSettings;

        public TcpServer(IClientStorage subscribersStorage, IOptions<TcpSettings> tcpSettings, IMessageProcessor messageProcessor)
        {
            _subscribersStorage = subscribersStorage;
            _messageProcessor = messageProcessor;

            if (tcpSettings == null)
                throw new ArgumentNullException(nameof(tcpSettings));

            _tcpSettings = tcpSettings.Value;
        }

        public async void StartListening(CancellationTokenSource cts)
        {
            if (cts == null)
                throw new ArgumentNullException(nameof(cts));

            try
            {
                if (!IPAddress.TryParse(_tcpSettings.Server, out var localAddress))
                {
                    Console.WriteLine("Attention for ip address.");
                    return;
                }

                _server = new TcpListener(localAddress, _tcpSettings.Port);
                _server.Start();
                Console.WriteLine("Server started.");

                while (!cts.IsCancellationRequested)
                {
                    var client = await _server.AcceptTcpClientAsync();

                    var tcpClient = new TcpChatClient(client, _messageProcessor, _subscribersStorage)
                    { Info = "Tcp:" + ((IPEndPoint)client.Client.RemoteEndPoint)?.Address };

                    var tcpClientThread = new Thread(tcpClient.Chatting);
                    tcpClientThread.Start();
                    _subscribersStorage.AddOrUpdateClient(tcpClient, tcpClient.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _server.Stop();

            foreach (var t in _subscribersStorage.ChatClients.Keys.Where(x => x is TcpChatClient))
            {
                t.Close();
            }
        }
    }
}
