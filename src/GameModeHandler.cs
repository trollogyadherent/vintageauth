using System;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace VintageAuth {
    public static class GameModeHandler {
        public static void ChangeMode(IServerPlayer player, EnumGameMode mode) {
            Console.WriteLine($"Assigning mode {mode} to {player.PlayerName}");
            if (!VintageAuth.serverMain.PlayerDataManager.WorldDataByUID.TryGetValue(player.PlayerUID, out var targetPlayerWorldData))
            {
                Console.WriteLine("Player never connected to this server. Must at least connect once to set game mode.");
                return;
            }
            EnumGameMode modeBefore = targetPlayerWorldData.GameMode;
            targetPlayerWorldData.GameMode = mode;
            bool canFreeMove = mode == EnumGameMode.Creative || mode == EnumGameMode.Spectator;
            targetPlayerWorldData.FreeMove = (targetPlayerWorldData.FreeMove && canFreeMove) || mode == EnumGameMode.Spectator;
            targetPlayerWorldData.NoClip = (targetPlayerWorldData.NoClip && canFreeMove) || mode == EnumGameMode.Spectator;
            if (mode == EnumGameMode.Survival || mode == EnumGameMode.Guest)
            {
                if (modeBefore == EnumGameMode.Creative)
                {
                    targetPlayerWorldData.PreviousPickingRange = targetPlayerWorldData.PickingRange;
                }
                targetPlayerWorldData.PickingRange = GlobalConstants.DefaultPickingRange;
            }
            if (mode == EnumGameMode.Creative && (modeBefore == EnumGameMode.Survival || modeBefore == EnumGameMode.Guest))
            {
                targetPlayerWorldData.PickingRange = targetPlayerWorldData.PreviousPickingRange;
            }
            ServerPlayer targetPlayer = (ServerPlayer) player;//VintageAuth.serverMain.GetConnectedClient(player.PlayerUID);
            if (mode != modeBefore)
            {
                if (targetPlayer != null)
                {
                    for (int i = 0; i < VintageAuth.serverSystem.Length; i++)
                    {
                        VintageAuth.serverSystem[i].OnPlayerSwitchGameMode(targetPlayer);
                    }
                }
                if (mode == EnumGameMode.Guest || mode == EnumGameMode.Survival)
                {
                    targetPlayerWorldData.MoveSpeedMultiplier = 1f;
                }
                if (targetPlayer != null)
                {
                    VintageAuth.serverMain.EventManager.TriggerPlayerChangeGamemode(targetPlayer);
                }
            }
            if (targetPlayer != null)
            {
                Console.WriteLine("Broadcasting gamemode change");
                VintageAuth.serverMain.BroadcastPlayerData(targetPlayer, sendInventory: false);
            }
        }
    }
}