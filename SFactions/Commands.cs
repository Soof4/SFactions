using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace SFactions {
    public class Commands {

        private static string[] subcommands = {"help", "change"}; 
        public static void FactionCmd(CommandArgs args) {
            TSPlayer player = args.Player;
            if(args.Parameters.Count < 1) {
                player.SendErrorMessage("You need to specify a subcommand. Do '/faction help' to see all subcommands.");
                return;
            }

            string subcmd = args.Parameters[0];
            if (subcommands.Contains(subcmd)) {
                if (!SFactionsMain.Config.subcommandsPerms[subcmd]) {
                    player.SendErrorMessage("You do not have permission to use this subcommand");
                    return;
                }
                switch (subcmd) {
                    case ("help"):
                        HelpCmd(args);
                        return;
                    case ("change"):
                        ChangeCmd(args);
                        return;
                }
            }
            player.SendErrorMessage("Subcommand could not be found.");
        }

        private static void ChangeCmd(CommandArgs args) {
            TSPlayer player = args.Player;
            int factionIndex;
            if (args.Parameters.Count < 2) {
                player.SendErrorMessage("You need to specify a the faction color.");
                return;
            }
            switch(args.Parameters[1]) {
                case ("red"):
                    factionIndex = 1;
                    break;
                case ("green"):
                    factionIndex = 2;
                    break;
                case ("blue"):
                    factionIndex = 3;
                    break;
                case ("yellow"):
                    factionIndex = 4;
                    break;
                case ("pink"):
                    factionIndex = 5;
                    break;
                default:
                    player.SendErrorMessage("Unknown faction color!");
                    return;
            }
            SFactionsMain.db.players[player.Name] = factionIndex;
            args.Player.SendSuccessMessage($"You've changed your faction to {args.Parameters[1].SpacedPascalCase()} successfully.");
            SFactionsMain.db.Write();
            PvPManager.ChangeTeam(args.Player);
        }

        private static void HelpCmd(CommandArgs args) {
            args.Player.SendErrorMessage("This subcommand has not been implemented yet.");
        }
    }
}
