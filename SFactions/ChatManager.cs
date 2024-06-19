using TShockAPI;
using TShockAPI.Configuration;
using TShockAPI.Hooks;

namespace SFactions {
    public static class ChatManager {
        
        public static void OnPlayerChat(PlayerChatEventArgs args) {
        
            // Format the message
            string message = string.Format(SFactions.Config.ChatFormat, 
                "",    // {0}
                args.Player.Group.Prefix,    // {1}
                args.Player.Name,    // {2}
                args.Player.Group.Suffix,    // {3}
                args.RawText,    // {4}
                SFactions.OnlineMembers.ContainsKey((byte)args.Player.Index) ? SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.Index]].Name : "");    // {5}
                

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
