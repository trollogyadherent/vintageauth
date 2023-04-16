using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using HarmonyLib;
using System;
using Vintagestory.Server;
using System.Drawing;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;


[assembly: ModInfo( "VintageAuth",
	Description = "Simple auth for offline Vintage Story servers.",
	Website     = "https://www.mcdrama.net/articles/mods.html",
	Authors     = new []{ "trollogyadherent" } )]

namespace VintageAuth
{
	public class VintageAuth : ModSystem
	{
		public static VAConfig vaConfig {get; set; } = new VAConfig();
		private static bool allowSaving = true;
		public static ICoreServerAPI serverApi;
		public static ICoreAPI commonApi;
		public static ServerMain serverMain;
		public static PlayerRole restrictedRole;

		public override bool ShouldLoad(EnumAppSide side)
        {
            return true;//side == EnumAppSide.Server;
        }

		public virtual double ExecuteOrder()
        {
            return 0;
        }

		public override void Start(ICoreAPI api)
		{
			commonApi = api;
		}
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			NetworkHandler.initClient(api);
			api.World.Logger.Event("Hello from VintageAuth client");
		}
		
		public override void StartServerSide(ICoreServerAPI api)
		{
			api.World.Logger.Notification($"Hello from VintageAuth ({VAConstants.MODID})!");
			Harmony harmony = new Harmony(VAConstants.MODID);
			base.StartServerSide(api);
			serverApi = api;
			allowSaving = LoadConfig();
			if(!DBhandler.InitDB()) {
				api.World.Logger.Error("Failed to open VintageAuth database! Not loading");
				return;
			}
			harmony.PatchAll();
			if (DBhandler.GetByUsername(vaConfig.admin_username) == null) {
				DBhandler.insertUser(vaConfig.admin_username, UuidUtil.uuidFromStr(vaConfig.admin_username), vaConfig.admin_password, true, "admin");
			}
			CommandHandler.registerCommands(api);
			NetworkHandler.initServer(api);
			api.Event.PlayerJoin += OnPlayerJoin;
			api.Event.RegisterGameTickListener(OnGameTickListener, 20);
		}

		private void OnPlayerJoin(IServerPlayer byPlayer)
        {
			Console.WriteLine("Player joined");
            Console.WriteLine(byPlayer);
			Console.WriteLine(byPlayer.ClientId);
			Console.WriteLine(byPlayer.PlayerName);
			Console.WriteLine(byPlayer.PlayerUID);
			Console.WriteLine(byPlayer.Role);
			byPlayer.SetRole(VAConstants.NOTLOGGEDROLE);

			/* Sends login command info depending if registration is open/closed/token only */
			if (vaConfig.registration_allowed) {
				if (vaConfig.token_needed) {
					byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.reg_usage_token, EnumChatType.Notification);
				} else {
					byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.reg_usage_no_token, EnumChatType.Notification);
				}
			} else {
				byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.reg_disabled_message, EnumChatType.Notification);
			}
			byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.login_usage.Replace("%time%", (VintageAuth.vaConfig.kick_unauthed_after).ToString()), EnumChatType.Notification);
			byPlayer.WorldData.CurrentGameMode = EnumGameMode.Guest;
			KickHandler.KickIfUnauthed(byPlayer);
        }

		/* teleports players back to spawn if they go too far unregistered */
		public void OnGameTickListener (float dt) {
			foreach (IServerPlayer player_ in serverApi.Server.Players) {
				//Console.WriteLine(fruit);
				if (player_.Role.Code.Equals(VAConstants.NOTLOGGEDROLE)) {
					player_.Entity.Controls.StopAllMovement();
					player_.Entity.ServerControls.StopAllMovement();
				}
				if (player_.Role.Code.Equals(VAConstants.NOTLOGGEDROLE) && player_.Entity.Pos.DistanceTo(serverApi.World.DefaultSpawnPosition) > vaConfig.max_unauthenticated_walk_distance) {
					player_.SendMessage(GlobalConstants.GeneralChatGroup, "Log in to move outside of spawn!", EnumChatType.CommandError);
					player_.Entity.TeleportTo(serverApi.World.DefaultSpawnPosition);
				}
			}
			
		}

		public static void addRestrictedRole() {
			restrictedRole = new PlayerRole{
				Code = VAConstants.NOTLOGGEDROLE,
				Name = "Not Logged In",
				Description = "VintageAuth role for unathenticated users.",
				PrivilegeLevel = -1,
				Color = Color.Brown,
				DefaultGameMode = EnumGameMode.Spectator,
				Privileges = new List<string>(new string[1] { VAConstants.VAPRIVILEGE })
				};
			serverMain.Config.RolesByCode[VAConstants.NOTLOGGEDROLE] = restrictedRole;
		}

		public static bool LoadConfig(ICoreServerAPI sapi)
        {
            try
            {
                if (sapi.LoadModConfig<VAConfig>(VAConstants.CONFIGNAME) == null) {
					sapi.World.Logger.Notification("LoadModConfig<VAConfig> is null");
					SaveConfig(sapi);
					return true;
				}

                vaConfig = sapi.LoadModConfig<VAConfig>(VAConstants.CONFIGNAME);
                SaveConfig(sapi);
                return true;
            }
            catch (Exception)
            {
                sapi.World.Logger.Notification($"Error while parsing {VAConstants.MODNAME} configuration file");//, will use fallback settings. All changes to configuration in game will not be saved!");
                //capi.World.Logger.Notification("Use .vshudforcesave to fix.");
                return false;
            }
        }

		public bool LoadConfig() => LoadConfig(serverApi);
		public static void SaveConfig(ICoreServerAPI capi, bool force = false)
        {
            if (allowSaving || force)
            {
                capi.StoreModConfig(vaConfig, VAConstants.CONFIGNAME);
                allowSaving = true;
            }
        }

		public static void Log(string message) {
			serverApi.World.Logger.Notification(message);
		}
	}
}
