using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Common.Messages.Enums;

namespace Common.Messages
{
    public interface IMessage
    {
        DateTimeOffset TimeFrom { get; set; }

        MessageType Type { get; }

        string Body { get; set; }
    }

    public class Message : IMessage
    {
        public Message(MessageType type)
        {
            Type = type;
        }

        public DateTimeOffset TimeFrom { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; }

        public string Name { get; set; }

        public string Body { get; set; }
    }
}