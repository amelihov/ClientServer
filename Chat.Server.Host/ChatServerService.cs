using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Chat.Server.Infrastructure;

namespace Chat.Server.Host
{
    public class ChatServerService : IHostedService, IDisposable
    {
        private readonly ServerProcessing _serverProcessing;

        private CancellationTokenSource _cts;

        private Task _executingTask;

        public ChatServerService(ServerProcessing serverProcessing)
        { 
            _serverProcessing = serverProcessing;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = new CancellationTokenSource();
            cancellationToken.Register(_cts.Cancel);
            _executingTask = ExecuteAsync();

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return Task.CompletedTask;

            _cts.Cancel();
            return _executingTask;
        }

        private async Task ExecuteAsync()
        {
            try
            {
                await _serverProcessing.StartProcessing(_cts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex} Service crashed.");
            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
        }
    }
}