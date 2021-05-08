using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Messages;
using Common.Messages.Enums;
using Newtonsoft.Json;

namespace Chat.Client.Infrastructure
{
    public interface IPublisher
    {
        Task PublishMessage(NetworkStream stream, DateTimeOffset sentTime, string bodyMessage, string nameClient, bool isSubscribe = false);
    }

    public class Publisher : IPublisher
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        public Publisher()
        {
            _jsonSerializerSettings = new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml };
        }
        public async Task PublishMessage(NetworkStream stream, DateTimeOffset sentTime, string bodyMessage, string nameClient, bool isSubscribe = false)
        {
            var message = new Message(isSubscribe ? MessageType.Subscribe : MessageType.Network)
            {
                Name = nameClient,
                TimeFrom = sentTime,
                Body = bodyMessage
            };

            await Publish(message, stream);
        }

        private async Task Publish(IMessage message, Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            var stringMessage = JsonConvert.SerializeObject(message, _jsonSerializerSettings);

            var data = Encoding.UTF8.GetBytes(stringMessage);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
