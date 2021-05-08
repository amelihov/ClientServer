using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Chat.Client.Host
{
    public class ChatClientService : IHostedService, IDisposable
    {
        private readonly IClientProcessing _clientProcessing;
        private CancellationTokenSource _cts;

        private Task _executingTask;

        public ChatClientService(IClientProcessing clientProcessing)
        {
            _clientProcessing = clientProcessing;
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
                await _clientProcessing.StartProcessing(_cts);
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
