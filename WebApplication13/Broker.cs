using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web.WebSockets;

namespace WebApplication13
{
    public class Broker
    {
        private static readonly ConcurrentDictionary<int, Task> sendingTasks = new ConcurrentDictionary<int, Task>();
        private static readonly object _lock = new object();

        private static ActionBlock<Tuple<AspNetWebSocketContext, string>> _actionBlock;
        private static ActionBlock<Tuple<AspNetWebSocketContext, string>> ActionBlock
        {
            get
            {
                return _actionBlock ?? (_actionBlock = new ActionBlock<Tuple<AspNetWebSocketContext, string>>(
                 t => SendAsync(t.Item1, t.Item2, 3),
                    new ExecutionDataflowBlockOptions()
                    {
                        MaxDegreeOfParallelism = 5
                    }));
            }
        }

        private static async Task SendAsync(AspNetWebSocketContext socketContext, string message, int retryCount)
        {
            var data = Encoding.UTF8.GetBytes(message + " => " + WebSocketCollection.Instance.Count.ToString());
            var arraySegment = new ArraySegment<byte>(data);
            try
            {
                var context = socketContext;
                var socket = context.WebSocket;
                await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (InvalidOperationException ex)
            {
                await Task.Delay(1000);
                if (retryCount > 0)
                {
                    await SendAsync(socketContext, message, --retryCount);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public static void Send(AspNetWebSocketContext context, string message)
        {
            ActionBlock.SendAsync(new Tuple<AspNetWebSocketContext, string>(context, message));
        }
    }
}