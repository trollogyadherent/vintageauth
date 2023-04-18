using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace VintageAuth {
    public static class NetworkHandler {
        private static ICoreServerAPI sapi = null;
        private static ICoreClientAPI capi = null;
        public static IClientNetworkChannel clientChannel;
        public static IServerNetworkChannel serverChannel;

        public static ClipboardServerHandler clipboardHandlerServer;

        public static void initServer(ICoreServerAPI serverApi) {
            serverChannel = serverApi.Network.RegisterChannel(VAConstants.VINTAGEAUTHNETWORKCHANNEL);
            sapi = serverApi;
            sapi.World.Logger.Event("Initializing VintageAuth NetworkManager (Server)");

            clipboardHandlerServer = new ClipboardServerHandler();
            clipboardHandlerServer.start();
            KeepLoginServerHandler keepLoginServerHandler = new KeepLoginServerHandler();
            keepLoginServerHandler.start();
            WelcomeServerHandler welcomeServerHandler = new WelcomeServerHandler();
            welcomeServerHandler.start();
        }

        public static void initClient(ICoreClientAPI clientApi) {
            clientChannel = clientApi.Network.RegisterChannel(VAConstants.VINTAGEAUTHNETWORKCHANNEL);
            capi = clientApi;
            capi.World.Logger.Event("Initializing VintageAuth NetworkManager (Client)");

            ClipboardClientHandler clipboardClientHandler = new ClipboardClientHandler();
            clipboardClientHandler.start(capi);
            KeepLoginClientHandler keepLoginClientHandler = new KeepLoginClientHandler();
            keepLoginClientHandler.start(capi);
            WelcomeClientHandler welcomeClientHandler = new WelcomeClientHandler();
            welcomeClientHandler.start(capi);
        }
    }
}