using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chat.Server.Storages;

namespace Chat.Server.Infrastructure
{
    public interface IPublisher
    {
        Task PublishNetworkMessage(string message, string clientId);
        Task PublishSubscribeMessage(string message, string clientId, string nameClient);
    }

    public class Publisher : IPublisher
    {
        private readonly IClientStorage _subscribersStorage;
        private readonly IMessageStorage _messageStorage;

        public Publisher(IClientStorage subscribersStorage, IMessageStorage messageStorage)
        {
            _subscribersStorage = subscribersStorage;
            _messageStorage = messageStorage;
        }

        public async Task PublishNetworkMessage(string message, string clientId)
        {
            await PublishMessage(message, clientId);
        }

        public async Task PublishSubscribeMessage(string message, string clientId, string nameClient)
        {
            var messages = _messageStorage.GetLastThirtyMessages().ToList();
            foreach (var snapshotMessage in messages)
                await PublishMessage(snapshotMessage, clientId, true);

            await PublishMessage(message, clientId);
        }

        private async Task PublishMessage(string message, string id, bool isSnapshot = false)
        {
            var data = Encoding.UTF8.GetBytes(message);
            var clients = isSnapshot
                ? _subscribersStorage.ChatClients.Where(x => x.Key.Id == id)
                : _subscribersStorage.ChatClients;
            foreach (var t in clients)
            {
                await t.Key.SendMessageAsync(message);
            }
        }
    }
}
