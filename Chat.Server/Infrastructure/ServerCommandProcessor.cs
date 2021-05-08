using System;
using System.Linq;
using System.Net;
using System.Threading;
using Chat.Server.Domain;
using Chat.Server.Storages;

namespace Chat.Server.Infrastructure
{
    public interface IServerCommandProcessor
    {
        void StartProcessing();
    }
    public class ServerCommandProcessor : IServerCommandProcessor
    {
        private readonly IClientStorage _subscribersStorage;
        private readonly ITcpServer _server;

        public ServerCommandProcessor(IClientStorage subscribersStorage, ITcpServer server)
        {
            _subscribersStorage = subscribersStorage;
            _server = server;
        }
        public void StartProcessing()
        {
            var receiveThread = new Thread(ReceiveCommand);
            receiveThread.Start();
        }

        private void ReceiveCommand()
        {
            while (true)
            {
                Console.WriteLine("Enter command: ");

                var message = Console.ReadLine();
                switch (message)
                {
                    case CommandsConstants.Exit:
                        _server.Disconnect();
                        Environment.Exit(0);
                        break;

                    case CommandsConstants.Ls:
                        {
                            if (!_subscribersStorage.ChatClients.Any())
                                Console.WriteLine($"No client's connections.");

                            foreach (var (key, _) in _subscribersStorage.ChatClients)
                                Console.WriteLine($"Client by Id=[{key.Id}] and name=[{key.Name}] has additional info=[{key.Info}]");

                            break;
                        }
                    default:
                        Console.WriteLine($"Unknown this command.");
                        break;
                }
            }
        }
    }
}
