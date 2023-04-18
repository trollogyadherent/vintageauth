using System;
using System.Data.Common;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace VintageAuth
{
    public class CommandHandler {
        public static void registerCommandsNewAPI(ICoreAPI api) {
            IChatCommand regCommand = api.ChatCommands.Create(VAConstants.REGCOMMAND);
            
        }

        [Obsolete]
        public static void registerCommands(ICoreServerAPI api) {
            api.RegisterCommand(VAConstants.VAHELPCOMMAND, "VintageAuth help command", "Usage: /vahelp [command]", 
            
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        string command = args.PopWord();
                        if (command == null) {
                            string available = $"{VAConstants.VAHELPCOMMAND}, {VAConstants.REGCOMMAND}, {VAConstants.LOGINCOMMAND}, {VAConstants.LOGOUTCOMMAND}, {VAConstants.CHANGEPWCOMMAND}, {VAConstants.DELACCOUNTCOMMAND}";
                            if (VintageAuth.vaConfig.roles_allowed_to_generate_tokens.Contains(player.Role.Code)) {
                                available += $", {VAConstants.GENTOKENCOMMAND}";
                            }
                            if (VintageAuth.vaConfig.roles_allowed_to_deactivate_accounts.Contains(player.Role.Code)) {
                                available += $", {VAConstants.DEACCOMMAND}, {VAConstants.REACCOMMAND}";
                            }
                            if (VintageAuth.vaConfig.roles_allowed_to_ban_accounts.Contains(player.Role.Code)) {
                                available += $", {VAConstants.VABANCOMMAND}";
                            }
                            if (VintageAuth.vaConfig.roles_allowed_to_change_account_roles.Contains(player.Role.Code)) {
                                available += $", {VAConstants.CHANGEROLECOMMAND}";
                            }
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Available commands: {available}", EnumChatType.CommandSuccess);
                            return;
                        }
                        
                        if (command.Equals(VAConstants.VAHELPCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.VAHELPCOMMAND} [command]». Used alone displays list of commands. Used with a command displays info about command.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.REGCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.REGCOMMAND} &lt;password&gt; [token]». Registers account with given password. The token is necessary only if token registration is enabled.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.LOGINCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.LOGINCOMMAND} &lt;password&gt;». Logs in current player.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.LOGINCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.LOGOUTCOMMAND} &lt;password&gt;». Logout. If you have the mod on your client, it won't auto-login again until next login.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.CHANGEPWCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.CHANGEPWCOMMAND} &lt;oldpassword&gt; &lt;newpassword&gt;». Changes password associated with current player.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.DELACCOUNTCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.DELACCOUNTCOMMAND} &lt;password&gt;». Deletes account associated with current player.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.GENTOKENCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.GENTOKENCOMMAND}». Generates a new one time use registration token.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.DEACCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.DEACCOMMAND} &lt;username&gt;». Deactivates given account, it may be reinstated later.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.REACCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.REACCOMMAND} &lt;username&gt;». Reactivates given account.", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.VABANCOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.VABANCOMMAND} &lt;username&gt;». Removes account from database. (you do it for free)", EnumChatType.CommandSuccess);
                        } else if (command.Equals(VAConstants.CHANGEROLECOMMAND)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Usage: «/{VAConstants.CHANGEROLECOMMAND} &lt;username&gt; &lt;role&gt;». Changes role of given account.", EnumChatType.CommandSuccess);
                        } else {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Unknown command: {command}", EnumChatType.CommandError);
                        }
                        
                    }, VAConstants.VAPRIVILEGE);
            
            api.RegisterCommand(VAConstants.REGCOMMAND, "VintageAuth register command", "Usage: /register &lt;password&gt; [token]", 
            
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        if (!VintageAuth.vaConfig.registration_allowed) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, VintageAuth.vaConfig.reg_disabled_message, EnumChatType.CommandError);
                            return;
                        }

                        User byUsername = DBhandler.GetByUsername(player.PlayerName);
                        User byUuid = DBhandler.GetByUuid(player.PlayerUID);
                        if (byUsername != null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Username '{byUsername.Name}' with UUID '{byUsername.Uuid}' already registered.", EnumChatType.CommandError);
                            return;
                        }

                        if (byUuid != null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"UUID '{byUuid.Uuid}' with username '{byUuid.Name}' already registered.", EnumChatType.CommandError);
                            return;
                        }
                        
                        if (VintageAuth.vaConfig.token_needed) {
                            if (args.Length != 2) {
                                player.SendMessage(GlobalConstants.GeneralChatGroup, "Usage: /register password token", EnumChatType.CommandError);
                                return;
                            }
                        } else {
                            if (args.Length != 1) {
                                player.SendMessage(GlobalConstants.GeneralChatGroup, "Usage: /register password", EnumChatType.CommandError);
                                return;
                            }
                        }
                        
                        string password = args.PopWord();
                        string token = args.PopWord();
                        if (password == null || (VintageAuth.vaConfig.token_needed && token == null)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                            return;
                        }
                        if (password.Length < VintageAuth.vaConfig.min_password_len) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"Password too short (min length is {VintageAuth.vaConfig.min_password_len} characters).", EnumChatType.CommandError);
                            return;
                        }

                        if (VintageAuth.vaConfig.token_needed) {
                            if (!DBhandler.isValidToken(token)) {
                                player.SendMessage(GlobalConstants.GeneralChatGroup, "Invalid token", EnumChatType.CommandError);
                            }
                        }

                        if (DBhandler.insertUser(player.PlayerName, player.PlayerUID, password, true, "")) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Registration successful!", EnumChatType.CommandSuccess);
                            if (VintageAuth.vaConfig.token_needed) {
                                DBhandler.removeToken(token);
                            }
                        } else {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                        }
                    }, VAConstants.VAPRIVILEGE);

        api.RegisterCommand(VAConstants.LOGINCOMMAND, "VintageAuth login command", "Usage: /login &lt;password&gt;", 
            
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        string errormsg = "Unknown username or incorrect password";
                        User byUsername = DBhandler.GetByUsername(player.PlayerName);
                        if (byUsername == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        
                        string password = args.PopWord();
                        if (password == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        if (!DBhandler.passwordValid(byUsername.Name, password)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }

                        if (!byUsername.Active) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Account deactivated", EnumChatType.CommandError);
                            return;
                        }

                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Login successful!", EnumChatType.CommandSuccess);
                        NetworkHandler.serverChannel.BroadcastPacket(new KeepLoginNetworkMessage(){message = password}, PlayerUtil.getRestrictedPlayers(player.PlayerName));
                        player.WorldData.CurrentGameMode = EnumGameMode.Survival;
                        //VintageAuth.serverMain.broadCastModeChange(player);
                        // give default role
                        if (byUsername.Role.Length == 0 || VintageAuth.serverMain.Config.RolesByCode[byUsername.Role] == null) {
                            Console.WriteLine($"Granting default role ({VintageAuth.serverMain.Config.DefaultRoleCode})");
                            player.SetRole(VintageAuth.serverMain.Config.DefaultRoleCode);
                            VintageAuth.serverMain.Config.RolesByCode[VintageAuth.serverMain.Config.DefaultRoleCode].GrantPrivilege(VAConstants.VAPRIVILEGE);
                        } else {
                            player.SetRole(byUsername.Role);
                            VintageAuth.serverMain.Config.RolesByCode[byUsername.Role].GrantPrivilege(VAConstants.VAPRIVILEGE);
                        }
                        VintageAuth.serverMain.BroadcastPlayerData(player, false);
                    }, VAConstants.VAPRIVILEGE);

        api.RegisterCommand(VAConstants.LOGOUTCOMMAND, $"VintageAuth {VAConstants.LOGOUTCOMMAND} command", $"Usage: /{VAConstants.LOGOUTCOMMAND}", 
            
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        
                    }, VAConstants.VAPRIVILEGE);


        api.RegisterCommand(VAConstants.CHANGEPWCOMMAND, "VintageAuth changepw command", "Usage: /changepw &lt;oldpw&gt; &lt;newpw&gt;", 
            
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        string errormsg = "Unknown username or incorrect password";
                        User byUsername = DBhandler.GetByUsername(player.PlayerName);
                        if (byUsername == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        
                        string password = args.PopWord();
                        if (password == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        if (!DBhandler.passwordValid(byUsername.Name, password)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        string newpassword = args.PopWord();
                        if (newpassword == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Incorrect syntax.", EnumChatType.CommandError);
                            return;
                        }
                        if (newpassword.Length < VintageAuth.vaConfig.min_password_len) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, $"New password too short (min length is {VintageAuth.vaConfig.min_password_len} characters).", EnumChatType.CommandError);
                            return;
                        }

                        if (DBhandler.updatePassword(player.PlayerName, newpassword)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Password update successful!", EnumChatType.CommandSuccess);
                            NetworkHandler.serverChannel.BroadcastPacket(new KeepLoginNetworkMessage(){message = password}, PlayerUtil.getRestrictedPlayers(player.PlayerName));
                        } else {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                        }
                    }, VAConstants.VAPRIVILEGE);
        
        api.RegisterCommand(VAConstants.DELACCOUNTCOMMAND, "VintageAuth delaccount command", "Usage: /delaccount &lt;password&gt;", 
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        string errormsg = "Unknown username or incorrect password";
                        User byUsername = DBhandler.GetByUsername(player.PlayerName);
                        if (byUsername == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        
                        string password = args.PopWord();
                        if (password == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }
                        if (!DBhandler.passwordValid(byUsername.Name, password)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, errormsg, EnumChatType.CommandError);
                            return;
                        }

                        if (DBhandler.deleteAccount(player.PlayerName)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Account deletion successful!", EnumChatType.CommandSuccess);
                            player.Disconnect();
                        } else {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                        }
                    }, VAConstants.VAPRIVILEGE);
        
        api.RegisterCommand(VAConstants.GENTOKENCOMMAND, "VintageAuth gentoken command", "Usage: /gentoken",   
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        if (!VintageAuth.vaConfig.roles_allowed_to_generate_tokens.Contains(player.Role.Code)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "You can't do that.", EnumChatType.CommandError);
                            return;
                        }
                        string token = DBhandler.addToken();
                        if (token == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                        }
                        
                        player.SendMessage(GlobalConstants.GeneralChatGroup, $"{token} (Install mod on client to receive tokens to clipboard)", EnumChatType.CommandSuccess);
                        NetworkHandler.serverChannel.BroadcastPacket(new NetworkClipboardMessage()
                            {
                                message = token,
                            }, PlayerUtil.getRestrictedPlayers(player.PlayerName));
                        
                    }, VAConstants.VAPRIVILEGE);

        api.RegisterCommand(VAConstants.DEACCOMMAND, "VintageAuth deactivate command", "Usage: /deactivate &lt;username&gt;", 
        (IServerPlayer player, int groupId, CmdArgs args) =>
                {
                    if (!VintageAuth.vaConfig.roles_allowed_to_deactivate_accounts.Contains(player.Role.Code)) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "You can't do that.", EnumChatType.CommandError);
                        return;
                    }

                    string username = args.PopWord();
                    if (username == null) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Invalid syntax.", EnumChatType.CommandError);
                        return;
                    }
                    if (DBhandler.GetByUsername(username) == null) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "User not found.", EnumChatType.CommandError);
                        return;
                    }
                    
                    foreach (IServerPlayer p in api.Server.Players)
                    {
                        if (p.PlayerName.Equals(username)) {
                            p.Disconnect(VintageAuth.vaConfig.deac_kick_message);
                            break;
                        }
                    }

                    if (DBhandler.deactivateAccount(username)) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Account deactivation successful!", EnumChatType.CommandSuccess);
                    } else {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                    }
                }, VAConstants.VAPRIVILEGE);

        api.RegisterCommand(VAConstants.REACCOMMAND, "VintageAuth activate command", "Usage: /activate &lt;username&gt;", 
        
        (IServerPlayer player, int groupId, CmdArgs args) =>
                {
                    if (!VintageAuth.vaConfig.roles_allowed_to_deactivate_accounts.Contains(player.Role.Code)) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "You can't do that.", EnumChatType.CommandError);
                        return;
                    }
                    
                    string username = args.PopWord();
                    if (username == null) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Invalid syntax.", EnumChatType.CommandError);
                        return;
                    }
                    if (DBhandler.GetByUsername(username) == null) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "User not found.", EnumChatType.CommandError);
                        return;
                    }

                    if (DBhandler.activateAccount(username)) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Account activation successful!", EnumChatType.CommandSuccess);
                    } else {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                    }
                }, VAConstants.VAPRIVILEGE);

        api.RegisterCommand(VAConstants.VABANCOMMAND, "VintageAuth vaban command", "Usage: /vaban &lt;username&gt;", 
        (IServerPlayer player, int groupId, CmdArgs args) =>
                {
                    if (!VintageAuth.vaConfig.roles_allowed_to_ban_accounts.Contains(player.Role.Code)) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "You can't do that.", EnumChatType.CommandError);
                        return;
                    }
                    
                    string username = args.PopWord();
                    if (username == null) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Invalid syntax.", EnumChatType.CommandError);
                        return;
                    }
                    if (DBhandler.GetByUsername(username) == null) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "User not found.", EnumChatType.CommandError);
                        return;
                    }

                    foreach (IServerPlayer p in api.Server.Players)
                    {
                        if (p.PlayerName.Equals(username)) {
                            p.Disconnect(VintageAuth.vaConfig.banned_kick_message);
                            break;
                        }
                    }
                    
                    if (DBhandler.deleteAccount(username)) {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Account deletion successful!", EnumChatType.CommandSuccess);
                    } else {
                        player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                    }
                }, VAConstants.VAPRIVILEGE);

        api.RegisterCommand(VAConstants.CHANGEROLECOMMAND, "VintageAuth changerole command", "Usage: /changerole &lt;username&gt; &lt;role&gt;",   
            (IServerPlayer player, int groupId, CmdArgs args) =>
                    {
                        if (!VintageAuth.vaConfig.roles_allowed_to_change_account_roles.Contains(player.Role.Code)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "You can't do that.", EnumChatType.CommandError);
                            return;
                        }

                        string username = args.PopWord();
                        string role = args.PopWord();

                        if (username == null || role == null) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Invalid syntax.", EnumChatType.CommandError);
                            return;
                        }

                        if (username.Equals(VintageAuth.vaConfig.admin_username)) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "You can't do that.", EnumChatType.CommandError);
                            return;
                        }

                        bool found  = false;
                        foreach (IPlayerRole role_ in VintageAuth.serverApi.Server.Config.Roles)
                        {
                           if (role_.Code.Equals(role)) {
                                found = true;
                                break;
                           }
                        }
                        if (!found) {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Role not found.", EnumChatType.CommandError);
                            return;
                        }

                        if (DBhandler.changeRole(username, role)) {
                            foreach (IServerPlayer p in api.Server.Players) {
                                if (p.PlayerName.Equals(username)) {
                                    p.SetRole(role);
                                    break;
                                }
                            }
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Role change successful!", EnumChatType.CommandSuccess);
                        } else {
                            player.SendMessage(GlobalConstants.GeneralChatGroup, "Error", EnumChatType.CommandError);
                        }
                    }, VAConstants.VAPRIVILEGE);
        }
    }
}