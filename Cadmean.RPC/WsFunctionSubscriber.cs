using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace Cadmean.RPC;

public class WsFunctionSubscriber : IFunctionSubscriber
{
    public async Task Subscribe<TResult>(string serverName, string functionName, Action<TResult> callback)
    {
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri($"ws://{serverName}/api/ws/rpcSubscribe");
        
        using var client = new WebsocketClient(url);
        
        client.MessageReceived.Subscribe(msg =>
        {
            Console.WriteLine("Message received: " + msg);
            // if (msg.ToString().ToLower() == "connected")
            // {
            //     string data = "{\"userKey\":\"" + streaming_API_Key + "\", \"symbol\":\"EURUSD,GBPUSD,USDJPY\"}";
            //     client.Send(data);
            // }
        });
        
        await client.Start();
        await client.SendInstant("pipipupu");
        await client.Stop(WebSocketCloseStatus.NormalClosure, "smal pipi");
        
        // exitEvent.WaitOne();
    }
}