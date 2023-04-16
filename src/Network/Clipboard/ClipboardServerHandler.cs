
using Vintagestory.API.Server;

namespace VintageAuth {
    public class ClipboardServerHandler {
        public IServerNetworkChannel serverChannel;
        ICoreServerAPI serverApi;

        public void start(ICoreServerAPI api)
        {
            serverApi = api;

            serverChannel =
                api.Network.RegisterChannel(VAConstants.NETWORKCLIPBOARDCHANNEL)
                .RegisterMessageType(typeof(NetworkClipboardMessage))
            ;
        }
    }
}