using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace PrintAPI.Middleware
{
    public interface IWebSocketServerConnectionManager
    {
        string AddSocket(WebSocket socket);
        ConcurrentDictionary<string, WebSocket> GetAllSockets();
    }
}