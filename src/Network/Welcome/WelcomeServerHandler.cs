namespace VintageAuth {
    public class WelcomeServerHandler {

        public void start()
        {
            NetworkHandler.serverChannel.RegisterMessageType(typeof(WelcomeNetworkMessage));
        }
    }
}