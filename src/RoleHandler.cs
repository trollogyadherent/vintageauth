using System;
using System.Collections.Generic;
using Vintagestory.API.Server;
using Vintagestory.Common;
using Vintagestory.Server;

namespace VintageAuth {
    public static class RoleHandler {
        public static void GrantRole(IServerPlayer player, string newRoleCode) {
            ServerMain server = VintageAuth.serverMain;
		ServerPlayerData targetPlayerData = server.PlayerDataManager.GetOrCreateServerPlayerData(player.PlayerUID, player.PlayerName);
		
		PlayerRole newRole = null;
		foreach (KeyValuePair<string, PlayerRole> val in server.Config.RolesByCode)
		{
			if (val.Key.ToLowerInvariant() == newRoleCode.ToLowerInvariant())
			{
				newRole = val.Value;
				break;
			}
		}
		if (newRole == null)
		{
            Console.WriteLine($"Role {newRoleCode} not found");
            return;
		}
		
		if (targetPlayerData == null)
		{
            Console.WriteLine($"No player with this playername found");
            return;
		}
		PlayerRole oldTargetRole = server.Config.RolesByCode[targetPlayerData.RoleCode];
		if (oldTargetRole.Code == newRole.Code)
		{
            Console.WriteLine($"Player is already in group {oldTargetRole.Code}");
			return;
		}
		targetPlayerData.SetRole(newRole);
		server.PlayerDataManager.playerDataDirty = true;
		ServerMain.Logger.Audit($"VintageAuth assigned {player.PlayerName} the role {newRole.Name}.");
		ConnectedClient client = server.GetClientByPlayername(player.PlayerName);
		if (client != null)
		{
			server.SendOwnPlayerData(player, sendInventory: false, sendPrivileges: true);
		} else {
            Console.WriteLine($"Failed to assign role, client null");
        }
        }

        public static PlayerRole roleidToRole(string id) {
            foreach (KeyValuePair<string, PlayerRole> val in VintageAuth.serverMain.Config.RolesByCode)
            {
                if (val.Key.ToLowerInvariant() == id.ToLowerInvariant())
                {
                    return val.Value;
                }
            }
            return (null);
        }
    }
}