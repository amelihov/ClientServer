using System;

namespace Common.Messages
{
    public record MessageIdentity
    {
        public MessageIdentity(DateTime timeFrom, string sender)
        {
            TimeFrom = timeFrom;
            Sender = sender;
        }

        public DateTime TimeFrom { get; }
        public string Sender { get; }
    }
}