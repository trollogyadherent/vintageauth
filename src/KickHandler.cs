using System;
using System.Threading;
using System.Threading.Tasks;
using Vintagestory.API.Server;

namespace VintageAuth {
    public class KickHandler {
        public static void KickIfUnauthed(IServerPlayer player)
        {
            Thread thread = new Thread(async () =>
            {
                VintageAuth.Log("Waiting in new thread...");
                await Task.Delay(TimeSpan.FromSeconds(VintageAuth.vaConfig.kick_unauthed_after));
                VintageAuth.Log($"Wait done (player role {player.Role.Name})...");
                if (player.Role.Code.Equals(VAConstants.NOTLOGGEDROLE))
                {
                    VintageAuth.Log($"Kicking unauthenticated player {player.PlayerName}");
                    player.Disconnect(VintageAuth.vaConfig.unauthed_kick_message.Replace("%time%", (VintageAuth.vaConfig.kick_unauthed_after).ToString()));
                }
            });
            thread.Start();
        }
    }
}