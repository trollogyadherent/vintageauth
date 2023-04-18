namespace VintageAuth
{
	public class VAConfig
	{
        public string database_name = "vintageauth_db.db";
        public bool registration_allowed = true;
        public bool token_needed = false;
        public string[] roles_allowed_to_generate_tokens = {"admin"};
        public string[] roles_allowed_to_deactivate_accounts = {"admin"};
        public string[] roles_allowed_to_ban_accounts = {"admin"};
        public string[] roles_allowed_to_change_account_roles = {"admin"};
        public string vahelp_message = "Use '/vahelp [command]' for VintageAuth (%ver%) help. Install the mod on your client to auto login.";
        public string reg_disabled_message = "Registration is disabled.";
        public string reg_usage_no_token = "Use '/register password' to create an account.";
        public string reg_usage_token = "Use '/register password invite_token' to create an account.";
        public string login_usage = "Use '/login password' to login in a span of %time% seconds.";
        public int min_password_len = 8;
        public int max_unauthenticated_walk_distance = 5;
        public string admin_username = VAConstants.DEFAULTADMINUSERNAME;
        public string admin_password = PasswordUtil.GenerateSecureHash(10);
        public int kick_unauthed_after = 60;
        public string unauthed_kick_message = "Login time exceeded (%time%) seconds.";
        public string deac_kick_message = "You have been deactivated ;_;";
        public string banned_kick_message = "You have been banned ;_;";
    }
}