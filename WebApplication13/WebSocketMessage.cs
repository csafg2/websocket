using System.Net.WebSockets;
using System.Web.WebSockets;

namespace WebApplication13
{

    public class WebSocketMessage
    { 
        public AspNetWebSocketContext Context { get; set; }
        public WebSocketMessageType MessageType { get; internal set; }
        public object Data { get; set; }
    }
}