using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Common.Messages;
using Newtonsoft.Json;

namespace Chat.Server.Storages
{
    public interface IMessageStorage
    {
        IEnumerable<string> GetLastThirtyMessages();
        void Add(NetworkMessage message);
    }

    public class MessageStorage : IMessageStorage
    {
        private readonly ConcurrentDictionary<MessageIdentity, string> _identityToMessage;

        public MessageStorage()
        {
            _identityToMessage = new ConcurrentDictionary<MessageIdentity, string>();
        }

        public IEnumerable<string> GetLastThirtyMessages()
        {
            return _identityToMessage
                    .OrderBy(x => x.Key.TimeFrom)
                    .Select(identity => identity.Value);
        }

        public void Add(NetworkMessage message)
        {
            var stringMessage = JsonConvert.SerializeObject(message);
            if (_identityToMessage.Count == 30)
            {
                var deletedMessage = _identityToMessage.OrderBy(x => x.Key.TimeFrom).First();
                _identityToMessage.Remove(deletedMessage.Key, out _);
            }
            _identityToMessage.AddOrUpdate(new MessageIdentity(DateTime.UtcNow, message?.Name), stringMessage, (_, _) => stringMessage);
        }
    }
}

