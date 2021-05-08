using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Messages.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageType
    {
        None,

        //_____________________________
        
        Network,
        Subscribe
    }
}