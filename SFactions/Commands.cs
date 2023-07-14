using IL.Terraria.GameContent;
using TShockAPI;
using SFactions.Database;

namespace SFactions {
    public class Commands {
        public static void FactionCmd(CommandArgs args) {
            TSPlayer player = args.Player;
            if(args.Parameters.Count < 1) {
                player.SendErrorMessage("You need to specify a subcommand. Do '/faction help' to see all subcommands.");
                return;
            }

            string subcmd = args.Parameters[0];
            
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
            if (!SFactions.onlineMembers.ContainsKey(args.Player.Name)) {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            Faction plrFaction = SFactions.onlineMembers[args.Player.Name];

            if (args.Player.Name.Equals(plrFaction.Leader)) {
                SFactions.onlineMembers.Remove(args.Player.Name);
                SFactions.dbManager.DeleteMember(args.Player.Name);
                plrFaction.Leader = null;
                SFactions.dbManager.SaveFaction(plrFaction);

            }
            else {
                SFactions.onlineMembers.Remove(args.Player.Name);
                SFactions.dbManager.DeleteMember(args.Player.Name);
            }

            args.Player.SendSuccessMessage("You've left your faction.");
        }

        private static void JoinCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            string factionName;

            if (SFactions.onlineMembers.ContainsKey(plr.Name)) {
                plr.SendErrorMessage("You need to leave your current faction first to join another one.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }
            factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            try {
                Faction newFaction = SFactions.dbManager.GetFaction(factionName);
                SFactions.onlineMembers.Add(plr.Name, newFaction);
                SFactions.dbManager.InsertMember(plr.Name, newFaction.Id);
                plr.SendSuccessMessage($"You've joined {newFaction.Name}.");
            }
            catch (NullReferenceException) {
                plr.SendErrorMessage($"There is no faction called {factionName}.");
            }
        }

        private static void CreateCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }

            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));
            if (factionName.Length < SFactions.Config.MinNameLength) {
                plr.SendErrorMessage($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
                return;
            }

            if (factionName.Length > SFactions.Config.MaxNameLength) {
                plr.SendErrorMessage($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
                return;
            }

            if (SFactions.onlineMembers.ContainsKey(args.Player.Name)) {
                plr.SendErrorMessage("You need to leave your current faction first to create new.\n" +
                    "If you want to leave your current faction do '/faction leave'");
                return;
            }

            try {
                SFactions.dbManager.GetFaction(factionName);
                plr.SendErrorMessage("A faction with this name already exists.");
                return;
            }
            catch (NullReferenceException) {
                SFactions.dbManager.InsertFaction(plr.Name, factionName);
                Faction newFaction = SFactions.dbManager.GetFaction(factionName);
                SFactions.dbManager.InsertMember(plr.Name, newFaction.Id);
                SFactions.onlineMembers.Add(plr.Name, newFaction);
                args.Player.SendSuccessMessage($"You've created {factionName}");
            }
        }

        private static void RenameCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.onlineMembers.ContainsKey(plr.Name)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineMembers[plr.Name];

            if (!plr.Name.Equals(SFactions.onlineMembers[plr.Name].Leader)) {
                plr.SendErrorMessage("Only leaders can change the faction name.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }

            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));
            if (factionName.Length < SFactions.Config.MinNameLength) {
                plr.SendErrorMessage($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
                return;
            }
            if (factionName.Length > SFactions.Config.MaxNameLength) {
                plr.SendErrorMessage($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
                return;
            }

            plrFaction.Name = factionName;
            SFactions.dbManager.SaveFaction(plrFaction);
            Utils.UpdateOnlineMembers();
        }

        private static void LeadCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if(!SFactions.onlineMembers.ContainsKey(plr.Name)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineMembers[plr.Name];

            if (plrFaction.Leader == null) {
                plrFaction.Leader = plr.Name;
                SFactions.dbManager.SaveFaction(plrFaction);
                Utils.UpdateOnlineMembers();
                plr.SendSuccessMessage("You're the leader of your faction now.");
            }
            else {
                plr.SendErrorMessage($"{plrFaction.Leader} is your faction's leader already.");
            }
        }
        
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
