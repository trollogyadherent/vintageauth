using Vintagestory.API.Client;

namespace VintageAuth {
    public class ClipboardClientHandler {
        IClientNetworkChannel clientChannel;
        ICoreClientAPI clientApi;

        public void start(ICoreClientAPI api)
        {
            clientApi = api;
            NetworkHandler.clientChannel.RegisterMessageType(typeof(NetworkClipboardMessage)).SetMessageHandler<NetworkClipboardMessage>(HandleResponse)
            ;
        }

        public void HandleResponse(NetworkClipboardMessage networkMessage) {
            if (networkMessage.message.Length < 100) {
                //ClipboardService.SetText(networkMessage.message);
                ClipboardHandler.CopyText(networkMessage.message);
            }
        }
    }
}