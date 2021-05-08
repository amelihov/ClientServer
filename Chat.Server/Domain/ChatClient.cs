using System;
using System.Threading.Tasks;

namespace Chat.Server.Domain
{
    public abstract class ChatClient
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Incognito";
        public string Info { get; set; }

        public abstract Task SendMessageAsync(string message);
        public abstract void Close();

    }
}
