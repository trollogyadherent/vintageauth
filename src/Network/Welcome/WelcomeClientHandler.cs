using System;
using Vintagestory.API.Client;

namespace VintageAuth {
    public class WelcomeClientHandler {
        
        ICoreClientAPI clientApi;

        public void start(ICoreClientAPI api)
        {
            clientApi = api;
            NetworkHandler.clientChannel.RegisterMessageType(typeof(WelcomeNetworkMessage)).SetMessageHandler<WelcomeNetworkMessage>(HandleResponse);
        }

        public void HandleResponse(WelcomeNetworkMessage networkMessage) {
            Console.WriteLine($"received welcome packet message {networkMessage.message}");
            string serverident = ServerCredHandler.connectDataToId(VintageAuth.serverConnectData);
            string password = ServerCredHandler.GetCredentials(serverident);
            if (password != null) {
                Console.WriteLine($"Got stored credentials for server {serverident}");
            } else {
                return;
            }
            Console.WriteLine($"sending /login {password}");
            clientApi.SendChatMessage($"/login {password}", 0);
        }
    }
}