using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Common.Converters;
using Common.Messages;
using Newtonsoft.Json;

namespace Common
{
    public interface ITcpMessagesStreamParser
    {
        IMessage GetNextMessage();
    }

    public class TcpMessagesStreamParser : ITcpMessagesStreamParser, IDisposable
    {
        private readonly JsonTextReader _jsonTextReader;

        private readonly StreamReader _streamReader;

        private bool _disposed;

        private readonly JsonSerializer _serializer;

        public TcpMessagesStreamParser(Stream messagesStream)
        {
            _streamReader = new StreamReader(messagesStream, bufferSize: 128);
            _jsonTextReader = new JsonTextReader(_streamReader) { SupportMultipleContent = true };
            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new MessageConverter());
        }

        public IMessage GetNextMessage()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TcpMessagesStreamParser));

            Message message;
            try
            {
                var tokenRead = _jsonTextReader.Read();

                if (!tokenRead)
                    return null;

                message = _serializer.Deserialize<Message>(_jsonTextReader);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to deserialize message: {e.Message}");
                throw new ArgumentNullException(nameof(message));
            }

            return message;
        }
        
        public void Dispose()
        {
            _disposed = true;
            ((IDisposable) _jsonTextReader)?.Dispose();
            _streamReader?.Dispose();
        }
    }
}