using ProtoBuf;

namespace VintageAuth {
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class KeepLoginNetworkMessage
    {
        public string message;
    }
}