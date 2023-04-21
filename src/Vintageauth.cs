using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using HarmonyLib;
using System;
using Vintagestory.Server;
using System.Drawing;
using System.Collections.Generic;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;
using System.Reflection;
using Vintagestory.Client;
using Vintagestory.Common;
using Vintagestory.API.Util;

[assembly: ModInfo( "VintageAuth",
	Description = "Simple auth for offline Vintage Story servers.",
	Website     = "https://www.mcdrama.net/articles/mods.html",
	Authors     = new []{ "trollogyadherent" },
	Side = "Universal", RequiredOnClient = false, RequiredOnServer = false )]

namespace VintageAuth
{
	public class VintageAuth : ModSystem
	{
		public static VAConfig vaConfig {get; set; } = new VAConfig();
		private static bool allowSaving = true;
		public static ICoreServerAPI serverApi;
		public static ICoreAPI commonApi;
		public static ServerMain serverMain;
		public static ClientMain clientMain;
		public static Vintagestory.Common.PlayerRole restrictedRole;
		public static Harmony harmony;
		public static ServerConnectData serverConnectData;
		public static Vintagestory.Client.NoObf.ClientPlatformAbstract platform;
		public static ServerSystem[] serverSystem;
		//Vintagestory.Common
	

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

		public override void Dispose()
        {
            base.Dispose();
            NetworkHandler.clientChannel = null;
			NetworkHandler.serverChannel = null;
            harmony.UnpatchAll(VAConstants.MODID);
        }
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			VAConstants.MODVERSION = api.ModLoader.GetMod(VAConstants.MODID).Info.Version;
			api.World.Logger.Event($"Hello from VintageAuth client ({VAConstants.MODID}, {VAConstants.MODVERSION})!");
			clientMain = (ClientMain) api.World;

			DoReflectionClient();
			
			/*harmony = new Harmony(VAConstants.MODID);
			MethodInfo minfoOriginalClient = AccessTools.Method(typeof(ClientSystemStartup), "OnAllAssetsLoaded_ClientSystems");
			if (minfoOriginalClient != null) {
				MethodInfo clientPrefix = SymbolExtensions.GetMethodInfo(() => ClientPatch.ClientPrefix());
				harmony.Patch(minfoOriginalClient, new HarmonyMethod(clientPrefix));
			} else {
				Console.WriteLine("Failed to hook client harmony, exiting.");
				return;
			}*/
			NetworkHandler.initClient(api);
			Console.WriteLine("REGISTERING HECKING PLAYERJOIN");
			api.Event.PlayerJoin += OnPlayerJoinClient;
			//api.Event.IsPlayerReady += OnPlayerJoinClient;
			//api.Event.PlayerEntitySpawn += OnPlayerJoinClient;
		}
		
		private void OnPlayerJoinClient(IClientPlayer byPlayer){
			Console.WriteLine($"Sum player joined client, {byPlayer}");
			if (clientMain == null) {
				return;
			}
			if (byPlayer.PlayerName.Equals(clientMain.Player.PlayerName)
			&& byPlayer.PlayerUID.Equals(clientMain.Player.PlayerUID)) {
				Console.WriteLine("This player is us!");
			} else {
				return;
			}
		}

		public override void StartServerSide(ICoreServerAPI api)
		{
			VAConstants.MODVERSION = api.ModLoader.GetMod(VAConstants.MODID).Info.Version;
			if (!api.Server.IsDedicated) {
				api.World.Logger.Event("VintageAuth: Non-dedicated server, exiting");
				return ;
			}
			api.World.Logger.Notification($"Hello from VintageAuth server ({VAConstants.MODID}, {VAConstants.MODVERSION})!");
			serverMain = (ServerMain)api.World;
			DoReflectionServer();
			//harmony = new Harmony(VAConstants.MODID);
			base.StartServerSide(api);
			serverApi = api;
			allowSaving = LoadConfig();
			if(!DBhandler.InitDB()) {
				api.World.Logger.Error("Failed to open VintageAuth database! Not loading");
				return;
			}
			//harmony.PatchAll();
			
			/*MethodInfo minfoOriginalServer = AccessTools.Method(typeof(ServerMain), "AfterSaveGameLoaded");
        	MethodInfo mPrefix = SymbolExtensions.GetMethodInfo((ServerMain __instance) => ServerPatch.ServerPrefix(__instance));
        	harmony.Patch(minfoOriginalServer, new HarmonyMethod(mPrefix), null);*/


			if (DBhandler.GetByUsername(vaConfig.admin_username) == null) {
				DBhandler.insertUser(vaConfig.admin_username, UuidUtil.uuidFromStr(vaConfig.admin_username), vaConfig.admin_password, true, "admin");
			}
			addRestrictedRole();
			CommandHandler.registerCommands(api);
			NetworkHandler.initServer(api);
			api.Event.PlayerNowPlaying += OnPlayerJoin;
			//api.Event.
			api.Event.RegisterGameTickListener(OnGameTickListener, 20);
		}

		public static void setClientMain() {
			Console.WriteLine("setClientMain running...");
			FieldInfo gameField = typeof(ClientSystemStartup).GetField("game", BindingFlags.NonPublic | BindingFlags.Instance);
			if (gameField == null) {
				VintageAuth.commonApi.Logger.Error("Failed to reflect 'game'");
				return;
			}
        	ClientMain clientMainInstance = (ClientMain)gameField.GetValue(ClientSystemStartup.instance);
			clientMain = clientMainInstance;
			Console.WriteLine("sneed: " + clientMain);

			FieldInfo connectDataField = ReflectionHelper.GetFieldInfo(typeof(ClientMain), "Connectdata");
			if (connectDataField == null) {
				VintageAuth.commonApi.Logger.Error("Failed to reflect 'Connectdata'");
				return;
			}
			serverConnectData = (ServerConnectData)connectDataField.GetValue(clientMain);
			//Console.WriteLine($"Got server data: {serverConnectData.Host}, {serverConnectData.HostRaw}, {serverConnectData.Port}");
		}

		private void OnPlayerJoin(IServerPlayer byPlayer)
        {
			Console.WriteLine("Player joined");
            Console.WriteLine(byPlayer);
			Console.WriteLine(byPlayer.ClientId);
			Console.WriteLine(byPlayer.PlayerName);
			Console.WriteLine(byPlayer.PlayerUID);
			Console.WriteLine(byPlayer.Role);
			//byPlayer.Privileges.Append(VAConstants.VAPRIVILEGE);
			ServerPlayerData plrdata = serverMain.PlayerDataManager.GetOrCreateServerPlayerData(byPlayer.PlayerUID, byPlayer.PlayerName);
			plrdata.GrantPrivilege(VAConstants.VAPRIVILEGE);
			serverMain.PlayerDataManager.playerDataDirty = true;
			serverMain.SendOwnPlayerData(byPlayer, false, true);

			RoleHandler.GrantRole(byPlayer, VAConstants.NOTLOGGEDROLE);
			/* Sends login command info depending if registration is open/closed/token only */
			byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.vahelp_message.Replace("%ver%", VAConstants.MODVERSION), EnumChatType.Notification);
			if (vaConfig.registration_allowed) {
				if (vaConfig.token_needed) {
					byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.reg_usage_token, EnumChatType.Notification);
				} else {
					byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.reg_usage_no_token, EnumChatType.Notification);
				}
			} else {
				byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.reg_disabled_message, EnumChatType.Notification);
			}
			GameModeHandler.ChangeMode(byPlayer, EnumGameMode.Spectator);
			byPlayer.SendMessage(GlobalConstants.GeneralChatGroup, vaConfig.login_usage.Replace("%time%", (VintageAuth.vaConfig.kick_unauthed_after).ToString()), EnumChatType.Notification);
			NetworkHandler.serverChannel.BroadcastPacket(new WelcomeNetworkMessage(){message = "welcome"}, PlayerUtil.getRestrictedPlayers(byPlayer.PlayerName));
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
			restrictedRole = new Vintagestory.Common.PlayerRole {
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

		static void DoReflectionClient() {
			FieldInfo connectDataField = ReflectionHelper.GetFieldInfo(typeof(ClientMain), "Connectdata");
			if (connectDataField == null) {
				VintageAuth.commonApi.Logger.Error("Failed to reflect 'Connectdata'");
			} else {
				serverConnectData = (ServerConnectData)connectDataField.GetValue(clientMain);
			}
			
			FieldInfo platformDataField = ReflectionHelper.GetFieldInfo(typeof(ClientMain), "Platform");
			if (platformDataField == null) {
				
				VintageAuth.commonApi.Logger.Error("Failed to reflect 'Connectdata'");
			} else {
				platform = (ClientPlatformAbstract)platformDataField.GetValue(clientMain);
			}
			
		}

		static void DoReflectionServer() {
			FieldInfo serverSystemField = ReflectionHelper.GetFieldInfo(typeof(ServerMain), "Systems");
			if (serverSystemField == null) {
				VintageAuth.commonApi.Logger.Error("Failed to reflect 'serverSystemField'");
			} else {
				serverSystem = (ServerSystem[])serverSystemField.GetValue(serverMain);
			}
		}
	}
}
