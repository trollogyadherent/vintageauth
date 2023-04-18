using System;
using Vintagestory.API.Client;

namespace VintageAuth {
    public class LogoutClientHandler {
        
        ICoreClientAPI clientApi;

        public void start(ICoreClientAPI api)
        {
            clientApi = api;
            NetworkHandler.clientChannel.RegisterMessageType(typeof(LogoutNetworkMessage)).SetMessageHandler<LogoutNetworkMessage>(HandleResponse);
        }

        public void HandleResponse(LogoutNetworkMessage networkMessage) {
            Console.WriteLine($"received logout packet message {networkMessage.message}, logging out");
            string serverident = ServerCredHandler.connectDataToId(VintageAuth.serverConnectData);
            ServerCredHandler.RemoveCredentials(serverident);
        }
    }
}