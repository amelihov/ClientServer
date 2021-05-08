using System;
using Common.Messages;
using Common.Messages.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chat.Server
{
    public class MessageConverter : JsonConverter<Message>
    {
        public override void WriteJson(JsonWriter writer, Message value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override Message ReadJson(JsonReader reader,
            Type objectType,
            Message existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var messageType = jObject["Type"].ToObject<MessageType>();

            return messageType switch
            {
                MessageType.Subscribe => jObject.ToObject<SubscribeMessage>(),
                MessageType.Network => jObject.ToObject<NetworkMessage>(),
                _ => null
            };
        }

        public override bool CanRead => true;
        public override bool CanWrite => false;
    }
}