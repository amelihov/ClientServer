using System.Threading;
using System.Threading.Tasks;
using Chat.Server.Domain;

namespace Chat.Server.Infrastructure
{
    public class ServerProcessing
    {
        private readonly ITcpServer _tcpServer;
        private readonly IServerCommandProcessor _serverCommandProcessor;

        public ServerProcessing(ITcpServer tcpServer, IServerCommandProcessor serverCommandProcessor)
        {
            _tcpServer = tcpServer;
            _serverCommandProcessor = serverCommandProcessor;
        }

        public Task StartProcessing(CancellationTokenSource cts = default)
        {
            _tcpServer.StartListening(cts);
            _serverCommandProcessor.StartProcessing();

            return Task.CompletedTask;
        }
    }
}
