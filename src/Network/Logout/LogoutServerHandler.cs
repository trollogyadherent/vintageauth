namespace VintageAuth {
    public class LogoutServerHandler {

        public void start()
        {
            NetworkHandler.serverChannel.RegisterMessageType(typeof(LogoutNetworkMessage));
        }
    }
}