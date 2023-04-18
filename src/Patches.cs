using System;
using System.Reflection;
using HarmonyLib;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

namespace VintageAuth
{
    [HarmonyPatch(typeof(ServerMain), "AfterSaveGameLoaded")]
    class ServerPatch
    {
        public static void ServerPrefix(ServerMain __instance)
        {
            Console.WriteLine("VintageAuth successfully hooked into server!");
            Console.WriteLine(__instance);
            VintageAuth.serverMain = __instance;
            VintageAuth.addRestrictedRole();
        }
    }

    [HarmonyPatch(typeof(ClientSystemStartup), "OnAllAssetsLoaded_ClientSystems")]
    class ClientPatch
    {
        public static void ClientPrefix()
        {
            Console.WriteLine("VintageAuth successfully hooked into client!");
            VintageAuth.setClientMain();
        }
    }
}