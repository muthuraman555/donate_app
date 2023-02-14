using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class HttpListenerContext
    {
        public object Request { get; internal set; }

        internal Task<WebSocketContext> AcceptWebSocketAsync(object subProtocol)
        {
            throw new NotImplementedException();
        }
    }
}