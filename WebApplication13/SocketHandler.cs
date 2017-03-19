using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace WebApplication13
{
    public class SocketHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(ProcessSocket);
            }
            else
            {
                context.Response.Write("Only WS");
                context.Response.End();
            }
        }

        private async Task ProcessSocket(AspNetWebSocketContext arg)
        {
            await SocketServer.Current.Add(arg);
        }
    }
}