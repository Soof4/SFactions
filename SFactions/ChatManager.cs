using TShockAPI;
using TShockAPI.Hooks;

namespace SFactions {
    public class ChatManager {
        
        public static void OnPlayerChat(PlayerChatEventArgs args) {
            args.Handled = true;

            if (args.RawText.StartsWith("/") || args.RawText.StartsWith(".")) {
                TShockAPI.Commands.HandleCommand(args.Player, args.RawText);
                return;
            }

            string username = args.Player.Name;
            string message = $"{username}: {args.RawText}";
            
            if (SFactionsMain.db.players.ContainsKey(username)) {
                message = $"[{SFactionsMain.db.factions[SFactionsMain.db.players[username]]}] {username}: {args.RawText}";
            }

            TSPlayer.All.SendMessage(message,
                new Microsoft.Xna.Framework.Color(
                args.Player.Group.R,
                args.Player.Group.G,
                args.Player.Group.B));

            TSPlayer.Server.SendConsoleMessage(message,
                args.Player.Group.R,
                args.Player.Group.G,
                args.Player.Group.B);
        }
    }
}
