using Vintagestory.API.Server;

public static class PlayerUtil {
    public static IServerPlayer[] getRestrictedPlayers(string name) {
        if (VintageAuth.VintageAuth.serverMain.AllOnlinePlayers.Length <= 1) {
            return new IServerPlayer[0];
        }
        IServerPlayer[] res = new IServerPlayer[VintageAuth.VintageAuth.serverMain.AllOnlinePlayers.Length - 1];
        int i = 0;
        foreach (IServerPlayer player in VintageAuth.VintageAuth.serverMain.AllOnlinePlayers) {
            if (!player.PlayerName.Equals(name)) {
                res[i] = player;
                i ++;
            }
        }
        return res;
    }
}