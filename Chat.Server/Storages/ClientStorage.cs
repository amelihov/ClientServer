using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Chat.Server.Domain;

namespace Chat.Server.Storages
{
    public interface IClientStorage
    {
        public ConcurrentDictionary<ChatClient, string> ChatClients { get; }
        bool AddOrUpdateClient(ChatClient chatClient, string id);
        bool RemoveClient(string id);
        ConcurrentDictionary<ChatClient, string> GetAll();
        ChatClient GetClientById(string id);
    }

    public class ClientStorage : IClientStorage
    {
        public ConcurrentDictionary<ChatClient, string> ChatClients { get; }
        public ClientStorage()
        {
            ChatClients = new ConcurrentDictionary<ChatClient, string>();
        }

        public bool AddOrUpdateClient(ChatClient chatClient, string id)
        {
            ChatClients.AddOrUpdate(chatClient, id, (_, _) => id);
            return true;
        }

        public bool RemoveClient(string id)
        {
            var (key, _) = ChatClients.FirstOrDefault(x => x.Key.Id == id);
            if (key != null)
                ChatClients.Remove(key, out _);

            return true;
        }

        public ConcurrentDictionary<ChatClient, string> GetAll()
        {
            return ChatClients;
        }

        public ChatClient GetClientById(string id)
        {
            return ChatClients.FirstOrDefault(p => p.Key.Id == id).Key;
        }
    }
}
