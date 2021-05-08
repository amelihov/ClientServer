using Microsoft.Extensions.Configuration;

namespace WebHostHelpers
{
    public class WebHostSettings
    {
        public KestrelSettings KestrelSettings { get; } = new KestrelSettings();
        
        public IConfiguration? ExtraConfiguration { get; set; }
    }
    
    public class KestrelSettings
    {
        public bool AllowSynchronousIO { get; set; }
    }
}