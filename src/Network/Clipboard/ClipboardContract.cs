using ProtoBuf;

namespace VintageAuth {
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class NetworkClipboardMessage
    {
        public string message;
    }
}