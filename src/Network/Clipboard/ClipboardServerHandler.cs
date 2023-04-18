namespace VintageAuth {
    public class ClipboardServerHandler {

        public void start()
        {
            NetworkHandler.serverChannel.RegisterMessageType(typeof(NetworkClipboardMessage));
        }
    }
}