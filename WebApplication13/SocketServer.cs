using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace WebApplication13
{

    public class SocketServer
    {

        private static SocketServer _current;

        private SocketServer() { }

        public static SocketServer Current
        {
            get
            {
                return _current ?? (_current = new SocketServer());
            }
        }


        public event EventHandler<MessageReceivedEventArgs> TextMessageReceived;

        public async Task Add(AspNetWebSocketContext context)
        {
            var buffer = WebSocket.CreateServerBuffer(4 * 1024);
            var id = Guid.NewGuid().ToString();

            var connection = new Connection()
            {
                Id = id,
                Context = context
            };

            WebSocketCollection.Instance.TryAdd(id, connection);

            await PrcessSocket(context, buffer, connection)
                .ContinueWith(t =>
                {
                    Connection ignore;
                    WebSocketCollection.Instance.TryRemove(id, out ignore);
                });


        }

        private async Task PrcessSocket(AspNetWebSocketContext context, ArraySegment<byte> buffer, Connection connection)
        {
            while (context.WebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await WebSocketMessageReader.ReceiveAsync(context.WebSocket, CancellationToken.None);
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            this.TextMessageReceived?.Invoke(connection.Context, new MessageReceivedEventArgs()
                            {
                                Message = new WebSocketMessage()
                                {
                                    MessageType = WebSocketMessageType.Text,
                                    Data = Encoding.UTF8.GetString((byte[])result.Data)
                                }
                            });
                            break;
                        case WebSocketMessageType.Binary:
                        case WebSocketMessageType.Close:
                            await context.WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        private async Task ReceiveTextAsync(Connection connection, WebSocketReceiveResult result, ArraySegment<byte> currentBuffer)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(4000);
            var message = Encoding.UTF8.GetString(currentBuffer.Array, 0, result.Count);
            while (!result.EndOfMessage)
            {
                result = await connection.Context.WebSocket.ReceiveAsync(currentBuffer, cts.Token);
                message += Encoding.UTF8.GetString(currentBuffer.Array, 0, result.Count);
            }

            if (this.TextMessageReceived != null)
            {
                this.TextMessageReceived(connection.Context.WebSocket, new MessageReceivedEventArgs()
                {
                    Message = new WebSocketMessage() { MessageType = WebSocketMessageType.Text, Data = message }
                });
            }

            //this.Send(message);
        }


    }



    public class WebSocketMessageReader
    {
        public static async Task<WebSocketMessage> ReceiveAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            var buffer = WebSocket.CreateServerBuffer(4 * 1024);

            var result = await socket.ReceiveAsync(buffer, cancellationToken);
            stream.Write(buffer.Array, 0, result.Count);

            while (!result.EndOfMessage)
            {
                result = await socket.ReceiveAsync(buffer, cancellationToken);
                stream.Write(buffer.Array, 0, result.Count);
            }

            return new WebSocketMessage
            {
                Data = stream.ToArray(),
                MessageType = result.MessageType
            };
        }
    }

}


