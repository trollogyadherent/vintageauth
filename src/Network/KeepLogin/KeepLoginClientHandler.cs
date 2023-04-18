using System;
using Vintagestory.API.Client;

namespace VintageAuth {
    public class KeepLoginClientHandler {
        
        ICoreClientAPI clientApi;

        public void start(ICoreClientAPI api)
        {
            clientApi = api;
            NetworkHandler.clientChannel.RegisterMessageType(typeof(KeepLoginNetworkMessage)).SetMessageHandler<KeepLoginNetworkMessage>(HandleResponse);
        }

        public void HandleResponse(KeepLoginNetworkMessage networkMessage) {
            //Console.WriteLine($"received login message {networkMessage.message}");
            Console.WriteLine($"Saving credentials data to {VAConstants.SERVERCREDSPATH}");
            ServerCredHandler.SaveCredentials(ServerCredHandler.connectDataToId(VintageAuth.serverConnectData), networkMessage.message);
        }
    }
}