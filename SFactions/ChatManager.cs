using IL.Microsoft.Xna.Framework;
using IL.Terraria;
using On.Terraria.GameContent.UI.ResourceSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
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
            int fPrefixIndex = SFactionsMain.db.players[username];

            TSPlayer.All.SendMessage($"{SFactionsMain.Config.factionPrefixes[fPrefixIndex]}{username}: {args.RawText}",
                new Microsoft.Xna.Framework.Color(
                args.Player.Group.R,
                args.Player.Group.G,
                args.Player.Group.B));

            TSPlayer.Server.SendConsoleMessage($"{SFactionsMain.Config.factionPrefixes[fPrefixIndex]}{username}: {args.RawText}",
                args.Player.Group.R,
                args.Player.Group.G,
                args.Player.Group.B);
        }
    }
}
