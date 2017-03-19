
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Web.WebSockets;

namespace WebApplication13
{
    public class SampleEchoService : IAppEvent
    {
        public AppEventType EvenType
        {
            get
            {
                return AppEventType.App_Start;
            }
        }

        public void Execute_AppHandler()
        {
            SocketServer.Current.TextMessageReceived += Current_TextMessageReceived;
        }

        private void Current_TextMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Send((string)e.Message.Data, e.Message.Context);
        }

        public void Send(string message, AspNetWebSocketContext context)
        {
            foreach (var item in WebSocketCollection.Instance.Values)
            {
                Broker.Send(item.Context, message);
            }
        }
    }

}