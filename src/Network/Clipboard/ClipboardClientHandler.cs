using Vintagestory.API.Client;

namespace VintageAuth {
    public class ClipboardClientHandler {
        IClientNetworkChannel clientChannel;
        ICoreClientAPI clientApi;

        public void start(ICoreClientAPI api)
        {
            clientApi = api;

            clientChannel =
                api.Network.RegisterChannel(VAConstants.NETWORKCLIPBOARDCHANNEL)
                .RegisterMessageType(typeof(NetworkClipboardMessage)).SetMessageHandler<NetworkClipboardMessage>(HandleResponse)
            ;
        }

        public void HandleResponse(NetworkClipboardMessage networkMessage) {
            clientApi.ShowChatMessage("Received following message from server: " + networkMessage.message);
            if (networkMessage.message.Length < 100) {
                //ClipboardService.SetText(networkMessage.message);
                ClipboardHandler.CopyText(networkMessage.message);
            }
        }
    }
}