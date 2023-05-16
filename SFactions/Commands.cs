using TShockAPI;

namespace SFactions {
    public class Commands {
        public static void FactionCmd(CommandArgs args) {
            TSPlayer player = args.Player;
            if(args.Parameters.Count < 1) {
                player.SendErrorMessage("You need to specify a subcommand. Do '/faction help' to see all subcommands.");
                return;
            }

            string subcmd = args.Parameters[0];
            if (!SFactionsMain.Config.subcommandsPerms[subcmd]) {
                player.SendErrorMessage("You do not have permission to use this subcommand.");
                return;
            }
            switch (subcmd) {
                case "help":
                    HelpCmd(args); return;
                case "create":
                    CreateCmd(args); return;
                case "join":
                    JoinCmd(args); return;
                case "leave":
                    LeaveCmd(args); return;
                case "rename":
                    RenameCmd(args); return;
                case "lead":
                    LeadCmd(args); return;
                
                default:
                    player.SendErrorMessage("Subcommand could not be found."); return;
            }
            
        }

        private static void LeaveCmd(CommandArgs args) {
            if (!SFactionsMain.db.players.ContainsKey(args.Player.Name)) {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            int playerFactionIndex = SFactionsMain.db.players[args.Player.Name];

            if (args.Player.Name.Equals(SFactionsMain.db.leaders[playerFactionIndex])) {
                SFactionsMain.db.leaders[playerFactionIndex] = "";
                SFactionsMain.db.players.Remove(args.Player.Name);
                foreach (var plr in TShock.Players) {
                    if (plr != null && SFactionsMain.db.players.ContainsKey(plr.Name) && (SFactionsMain.db.players[plr.Name] == playerFactionIndex)) {
                        plr.SendWarningMessage("Your leader has left the faction, your faction no longer has a leader.");
                    }
                }
            }
            else {
                SFactionsMain.db.players.Remove(args.Player.Name);
            }

            args.Player.SendSuccessMessage("You've left your faction.");
            SFactionsMain.db.Write();
        }

        private static void JoinCmd(CommandArgs args) {
            TSPlayer player = args.Player;
            string factionName = Utils.GetFactionName(args);

            if (SFactionsMain.db.players.ContainsKey(player.Name)) {
                player.SendErrorMessage("You need to leave your current faction to join another one.");
                return;
            }

            if (args.Parameters.Count < 2) {
                player.SendErrorMessage("You need to specify a the faction name.");
                return;
            }
            if (!(Utils.FindFactionName(factionName) != -1)) {
                player.SendErrorMessage($"There is no faction called {factionName}.");
                return;
            }

            int newFactionIndex = Utils.FindFactionName(factionName);

            SFactionsMain.db.players.Add(player.Name, newFactionIndex);
            args.Player.SendSuccessMessage($"You've joined to {factionName} faction.");

            SFactionsMain.db.Write();
        }

        private static void CreateCmd(CommandArgs args) {
            string newFactionName = Utils.GetFactionName(args);

            if (SFactionsMain.db.players.ContainsKey(args.Player.Name)) {
                args.Player.SendErrorMessage("You can't create new faction while in a faction.\n" +
                    "If you want to leave your current faction do '/faction leave'");
                return;
            }
            if (Utils.FindFactionName(newFactionName) != -1) {
                args.Player.SendErrorMessage("A faction with this name already exists.");
                return;
            }
            if (newFactionName.Length > SFactionsMain.Config.maxNameLength) {
                args.Player.SendErrorMessage($"Can't create the faction. Faction name is longer than {SFactionsMain.Config.maxNameLength}.");
                return;
            }

            int newFactionIndex = SFactionsMain.db.factions.Count;

            SFactionsMain.db.factions.Add(newFactionIndex, newFactionName);
            SFactionsMain.db.players.Add(args.Player.Name, newFactionIndex);
            SFactionsMain.db.leaders.Add(newFactionIndex, args.Player.Name);
            args.Player.SendSuccessMessage($"You've created {newFactionName} faction.");

            SFactionsMain.db.Write();
        }

        private static void RenameCmd(CommandArgs args) {
            if (!SFactionsMain.db.players.ContainsKey(args.Player.Name)) {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            int playerFactionIndex = SFactionsMain.db.players[args.Player.Name];

            if (!args.Player.Name.Equals(SFactionsMain.db.leaders[playerFactionIndex])) {
                args.Player.SendErrorMessage("Only leaders can change the faction name.");
                return;
            }

            string newName = Utils.GetFactionName(args);

            if (newName.Equals("")) {
                args.Player.SendErrorMessage("No name were given.");
                return;
            }
            if (newName.Length > SFactionsMain.Config.maxNameLength) {
                args.Player.SendErrorMessage($"Can't rename the faction. Faction name is longer than {SFactionsMain.Config.maxNameLength}.");
            }

            SFactionsMain.db.factions[playerFactionIndex] = newName;
            args.Player.SendSuccessMessage($"Your faction's name is {newName} now.");
            SFactionsMain.db.Write();
        }

        private static void LeadCmd(CommandArgs args) {
            if(!SFactionsMain.db.players.ContainsKey(args.Player.Name)) {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            int playerFactionIndex = SFactionsMain.db.players[args.Player.Name];
            
            if (SFactionsMain.db.leaders[playerFactionIndex].Equals("")) {
                SFactionsMain.db.leaders[playerFactionIndex] = args.Player.Name;
                SFactionsMain.db.Write();
                args.Player.SendSuccessMessage("You're the leader of your faction now.");
            }
            else {
                args.Player.SendErrorMessage($"{SFactionsMain.db.leaders[playerFactionIndex]} is your faction's leader already.");
            }
        }
        

        /*
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

            int preFactionIndex = SFactionsMain.db.players[player.Name];
            SFactionsMain.db.players[player.Name] = factionIndex;
            args.Player.SendSuccessMessage($"You've changed your faction to {args.Parameters[1].SpacedPascalCase()} successfully.");
            PvPManager.ChangeTeam(args.Player);

            if (preFactionIndex != 0 && SFactionsMain.db.leaders[preFactionIndex] == player.Name) {
                SFactionsMain.db.leaders[preFactionIndex] = "";
                foreach(var plr in TShock.Players) {
                    if (plr != null && (SFactionsMain.db.players[plr.Name] == preFactionIndex)) {
                        plr.SendWarningMessage("Your leader has left the faction, your faction no longer has a leader.");
                    }
                }
            }

            SFactionsMain.db.Write();
        }
        */

        private static void HelpCmd(CommandArgs args) {
            args.Player.SendInfoMessage("Subcommands:"
                + "\nhelp: Shows this message."
                + "\ncreate: Create a new faction (usage: /faction create <faction name>)"
                + "\njoin: Join a faction: (usage: /faction join <faction name>)"
                + "\nleave: Leave your faction"
                + "\nrename: Changes faction name, requires leadership permissions. (usage: /faction rename <name>)"
                + "\nlead: Make yourself the leader of your faction if there isn't someone else already."
                );
        }
    }
}
