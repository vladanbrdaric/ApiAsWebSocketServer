using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PrintAPI.Middleware
{
    public class WebSocketServerConnectionManager : IWebSocketServerConnectionManager
    {
        // This is kind of list (collection) of the sockets. So the "Key" will be ConnID and the "Value" will be "socket".
        private ConcurrentDictionary<string, WebSocket> _socket = new ConcurrentDictionary<string, WebSocket>();

        // Return all the sockets of connected clients so that I can broadcast a message to all the clients.
        // NOTE: I think that in my case I don't need this because it's gonna be only one client.
        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _socket;
        }

        // This method adds a new socket to the this collection of sockets "_socket", but first it creates an GUID so that each client has unique GUID.
        // It returns that GUID. In my case I think that I only need to create that GUID.
        public string AddSocket(WebSocket socket)
        {
            string ConnID = Guid.NewGuid().ToString();

            // ConnectionID is a "key" and the "value" is socket.
            _socket.TryAdd(ConnID, socket);
            Console.WriteLine("");
            Console.WriteLine($"Connection ID: { ConnID }");
            Console.WriteLine("");

            return ConnID;
        }
    }
}
