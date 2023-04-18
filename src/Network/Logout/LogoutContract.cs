using ProtoBuf;

namespace VintageAuth {
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class LogoutNetworkMessage
    {
        public string message;
    }
}