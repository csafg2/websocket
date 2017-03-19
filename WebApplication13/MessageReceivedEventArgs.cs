using System;

namespace WebApplication13
{
    public class MessageReceivedEventArgs: EventArgs
    {
        public WebSocketMessage Message { get; set; }
    }
}