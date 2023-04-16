using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace VintageAuth {
    public static class NetworkHandler {
        private static ICoreServerAPI sapi = null;
        private static ICoreClientAPI capi = null;

        public static ClipboardServerHandler clipboardHandlerServer;

        public static void initServer(ICoreServerAPI serverApi) {
            sapi = serverApi;
            sapi.World.Logger.Event("Initializing VintageAuth NetworkManager (Server)");
            clipboardHandlerServer = new ClipboardServerHandler();
            clipboardHandlerServer.start(sapi);
        }

        public static void initClient(ICoreClientAPI clientApi) {
            capi = clientApi;
            capi.World.Logger.Event("Initializing VintageAuth NetworkManager (Client)"); 
            ClipboardClientHandler clipboardClientHandler = new ClipboardClientHandler();
            clipboardClientHandler.start(capi);
        }
    }
}