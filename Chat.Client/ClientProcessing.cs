using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Client.Infrastructure;

namespace Chat.Client
{
    public interface IClientProcessing
    {
        Task StartProcessing(CancellationTokenSource tokenSource = default);
    }

    public class ClientProcessing : IClientProcessing
    {
        private readonly ITcpChatClient _tcpChatClient;

        public ClientProcessing(ITcpChatClient tcpChatClient)
        {
            _tcpChatClient = tcpChatClient;
        }

        public async Task StartProcessing(CancellationTokenSource tokenSource = default)
        {
            await _tcpChatClient.StartChattingAsync(tokenSource);
        }
    }
}
