using System;
using ProtoBuf;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace VintageAuth {
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class NetworkClipboardMessage
    {
        public string message;
    }
}