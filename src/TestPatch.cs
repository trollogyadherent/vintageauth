using System;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.Server;
using VintageAuth;

namespace VintageAuth
{
[HarmonyPatch(typeof(ServerMain), "AfterSaveGameLoaded")]
class TestPatch
{
    static void Prefix(ServerMain __instance)
    {
        Console.WriteLine("Sneed writing from hook!");
        Console.WriteLine(__instance);
        VintageAuth.serverMain = __instance;
        VintageAuth.addRestrictedRole();
    }
}
}