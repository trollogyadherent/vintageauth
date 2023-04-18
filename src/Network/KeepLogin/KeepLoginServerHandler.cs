namespace VintageAuth {
    public class KeepLoginServerHandler {

        public void start()
        {
            NetworkHandler.serverChannel.RegisterMessageType(typeof(KeepLoginNetworkMessage));
        }
    }
}