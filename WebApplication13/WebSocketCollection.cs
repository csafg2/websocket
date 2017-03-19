using System.Collections.Concurrent;
using System.Web.WebSockets;

namespace WebApplication13
{

    public class WebSocketCollection : ConcurrentDictionary<string, Connection>
    {
        private static WebSocketCollection _instance;

        public static WebSocketCollection Instance
        {
            get
            {
                return _instance ?? (_instance = new WebSocketCollection());
            }
        }
    }
}