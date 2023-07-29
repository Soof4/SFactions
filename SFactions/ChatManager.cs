using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace SFactions {
    public class ChatManager {
        
        public static void OnPlayerChat(PlayerChatEventArgs args) {

            // Handle commands
            if (args.RawText.StartsWith(TShock.Config.Settings.CommandSpecifier) || args.RawText.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)) {
                TShockAPI.Commands.HandleCommand(args.Player, args.RawText);
                return;
            }

            // Format the message
            string message = string.Format(SFactions.Config.ChatFormat, "", 
                args.Player.Group.Prefix, 
                args.Player.Name,
                args.Player.Group.Suffix,
                args.RawText,
                SFactions.onlineMembers.ContainsKey((byte)args.Player.Index) ? $"[{SFactions.onlineFactions[SFactions.onlineMembers[(byte)args.Player.Index]].Name}] " : "");
                

            TSPlayer.All.SendMessage(message,
                new Microsoft.Xna.Framework.Color(
                args.Player.Group.R,
                args.Player.Group.G,
                args.Player.Group.B));

            TSPlayer.Server.SendConsoleMessage(message,
                args.Player.Group.R,
                args.Player.Group.G,
                args.Player.Group.B);

            args.Handled = true;
        }
    }
}
