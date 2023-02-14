using System.Threading.Tasks;

namespace System.Net
{
    internal class WebSocketServer
    {
        private string v;

        public WebSocketServer(string v)
        {
            this.v = v;
        }

        internal Task StartAsync(Func<object, object, Task> p)
        {
            throw new NotImplementedException();
        }
    }
}