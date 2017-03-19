using System.Net.WebSockets;
using System.Web.WebSockets;

namespace WebApplication13
{

    public class Connection
    {
        public string Id { get; set; }
        public AspNetWebSocketContext Context { get; set; }
    }
}