using Vintagestory.API.Config;
using System.IO;

namespace VintageAuth
{
	public class VAConstants
	{
        public readonly static string MODID = "vintageauth";
        public static string MODVERSION = "%MODVERSION%";
        public readonly static string MODNAME = "Vintageauth";
        public readonly static string CONFIGNAME = MODID + ".json";
        public readonly static string SERVERCREDSPATH = Path.Combine(GamePaths.ModConfig, "va_servers.json");
        public readonly static string NOTLOGGEDROLE = "notloggedin";
        public readonly static string VAHELPCOMMAND = "vahelp";
        public readonly static string REGCOMMAND = "register";
        public readonly static string LOGINCOMMAND = "login";
        public readonly static string CHANGEPWCOMMAND = "changepw";
        public readonly static string DELACCOUNTCOMMAND = "delaccount";
        public readonly static string GENTOKENCOMMAND = "gentoken";
        public readonly static string DEACCOMMAND = "deactivate";
        public readonly static string REACCOMMAND = "reactivate";
        public readonly static string VABANCOMMAND = "vaban";
        public readonly static string CHANGEROLECOMMAND = "changerole";
        public readonly static string GETUSERCOMMAND = "getuser";
        public readonly static string LOGOUTCOMMAND = "logout";
        public readonly static string VAPRIVILEGE = "vaprivilege";
        public readonly static string VINTAGEAUTHNETWORKCHANNEL = "va_channel";
        public readonly static string DEFAULTADMINUSERNAME = "va_admin";
    }
}