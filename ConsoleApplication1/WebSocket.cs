using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class WebSocket
    {
        public object State { get; internal set; }

        internal Task SendAsync(ArraySegment<byte> segment, object text, bool v, CancellationToken none)
        {
            throw new NotImplementedException();
        }

        internal Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken none)
        {
            throw new NotImplementedException();
        }
    }
}