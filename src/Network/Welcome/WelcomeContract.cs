using ProtoBuf;

namespace VintageAuth {
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class WelcomeNetworkMessage
    {
        public string message;
    }
}