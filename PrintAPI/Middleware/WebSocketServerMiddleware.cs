using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PrintAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrintAPI.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketServerConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, IWebSocketServerConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        // This methid will actually keep the connection open. It starts in "Startup.cs" on line 46.
        // In file WebSocketServerMiddlewareExtensions created I an appBuilder "UseWebSocketServer" where I 
        // returning this "WebSocketServerMiddleware".
        public async Task InvokeAsync(HttpContext context)
        {
            // Checking to see if it's a WebSocket Request
            if(context.WebSockets.IsWebSocketRequest)
            {

                // If so, create a WebSocket object and WebSocket connection is going to be established.
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("");
                Console.WriteLine(">>> Agent Connected <<<");

                // Generate a new ConnID (guid)
                string ConnID = _manager.AddSocket(webSocket);

                // Type 'send' to send ip address to the Agent.
                Console.Write("Type 'Send' to send the ip address to the Agent: ");
                Console.ReadLine();

                // ---------------------------------------------------------------------
                // 1) Send IP address to the agent
                string ipAddress = "10.10.13.10";


                // Send printJobString to the client.
                await SendToAgent(webSocket, ipAddress);

                // --------------------------------------------------------------------
                // 2) Receive the ping response from the Agent.
                await ReceiveAnswerFromAgent(webSocket, async (result, buffer) =>
                {
                    // NOTE: Here I can choose between "Text", "Close" and "Binary".
                    if(result.MessageType == WebSocketMessageType.Text)
                    {
                        // Convert the answer from bytes to text
                        var answer = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                        // Write the answer to the console.
                        Console.WriteLine($"Ping status from the Agent: { answer }");

                        return;
                    }
                    // Allow client to close the socket on a nice way.
                    else if(result.MessageType == WebSocketMessageType.Close)
                    {
                        // Write to the console that the client choosed to close the connection.
                        Console.WriteLine("Received 'Close' message.");

                        // Close a connection to the client.
                        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                        return;
                    }
                });
            }
        }

        // Send the printJob to the client.
        private async Task SendToAgent(WebSocket socket, string printJobString)
        {
            // Convert string to binary
            var buffer = Encoding.UTF8.GetBytes(printJobString);

            // Send a message to the client.
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        // Receive the answer from the Agent.
        private async Task ReceiveAnswerFromAgent(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
