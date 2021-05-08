
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.Server.Infrastructure;
using Chat.Server.Storages;
using Common.Messages;
using Common.Messages.Enums;
using Newtonsoft.Json;

namespace Chat.Server.Domain
{
    public interface IMessageProcessor
    {
        Task ProcessMessage(IMessage message, TcpChatClient tcpChatClient);
    }
    
    public class MessageProcessor : IMessageProcessor
    {
        private readonly IPublisher _publisher;
        private readonly IMessageStorage _messageStorage;

        public MessageProcessor(IPublisher publisher, IMessageStorage messageStorage)
        {
            _publisher = publisher;
            _messageStorage = messageStorage;
        }

        public Task ProcessMessage(IMessage message, TcpChatClient tcpChatClient)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (tcpChatClient == null)
                throw new ArgumentNullException(nameof(tcpChatClient));

            return message.Type switch
            {
                MessageType.Subscribe => ProcessNewClient(message as SubscribeMessage, tcpChatClient),
                MessageType.Network => ProcessNetworkMessage(message as NetworkMessage, tcpChatClient),
                MessageType.None => throw new ArgumentOutOfRangeException(nameof(message.Type)),
                _ => throw new KeyNotFoundException($"Key '{message.Type}' not found inside of the list of type of the messages.")
            };
        }
        
        private async Task ProcessNetworkMessage(NetworkMessage message, TcpChatClient tcpChatClient)
        {
            _messageStorage.Add(message);
            
            var stringMessage = JsonConvert.SerializeObject(message);
            
            await _publisher.PublishNetworkMessage(stringMessage, tcpChatClient.Id);
        }

        private async Task ProcessNewClient(SubscribeMessage message, TcpChatClient tcpChatClient)
        {
            tcpChatClient.Name = message.Name;

            var networkMessage = new NetworkMessage()
            {
                Body = $"{tcpChatClient.Name} connected to chat.",
                Name = tcpChatClient.Name,
                TimeFrom = message.TimeFrom
            };

            var stringMessage = JsonConvert.SerializeObject(networkMessage);

            await _publisher.PublishSubscribeMessage(stringMessage, tcpChatClient.Id, tcpChatClient.Name);
        }
    }
}