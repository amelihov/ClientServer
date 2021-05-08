using System;
using Common.Messages;
using Common.Messages.Enums;

namespace Common.Messages
{ 
    public class NetworkMessage : Message
    {
        public NetworkMessage() : base(MessageType.Network)
        {
        }
    };
}
