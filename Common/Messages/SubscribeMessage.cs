using Common.Messages.Enums;

namespace Common.Messages
{
    public class SubscribeMessage : Message
    {
        public SubscribeMessage() : base(MessageType.Subscribe)
        {
        }
    }
}
