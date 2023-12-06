using TShockAPI;
using SFactions.Database;
using Abilities;
using TShockAPI.Configuration;

namespace SFactions {
    public class Commands {
        private static Dictionary<string, Faction> Invitations = new();
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
                case "ability":
                    AbilityCmd(args); return;
                case "region":
                    RegionCmd(args); return;
                case "invitetype":
                    InviteTypeCmd(args); return;
                case "invite":
                    InviteCmd(args); return;
                case "accept":
                    AcceptCmd(args); return;
                case "info":
                    InfoCmd(args); return;
                default:
                    player.SendErrorMessage("Subcommand could not be found."); return;
            }
        }

        private static void AcceptCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You need to leave your current faction to join another.");
                return;
            }
            
            if (!Invitations.TryGetValue(plr.Name, out Faction? newFaction) || newFaction == null) {
                plr.SendErrorMessage("Coudln't find a pending invitation.");
                return;
            }

            // Add player to the faction
            SFactions.OnlineMembers.Add((byte)plr.Index, newFaction.Id);
            SFactions.DbManager.InsertMember(plr.Name, newFaction.Id);

            if (!SFactions.OnlineFactions.ContainsKey(newFaction.Id)) {
                SFactions.OnlineFactions.Add(newFaction.Id, newFaction);
            }

            RegionManager.AddMember(plr);
            plr.SendSuccessMessage($"You've joined {newFaction.Name}.");

        }

        private static void InfoCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("Please specify a faction name.");
                return;
            }

            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (!SFactions.DbManager.DoesFactionExist(factionName)) {
                plr.SendErrorMessage($"There is no faction called {factionName}.");
                return;
            }
            
            Faction faction = SFactions.DbManager.GetFaction(factionName);
            plr.SendInfoMessage($"Faction ID: {faction.Id}\n" +
                                $"Faction Name: {faction.Name}\n" +
                                $"Faction Leader: {faction.Leader}\n" +
                                $"Faction Ability: {faction.Ability}\n" +
                                $"Has a Region: {faction.Region != null}\n" +
                                $"Invite Type: {faction.InviteType}"
                                );
        }

        private static void InviteCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];
            
            if (plrFaction.InviteType == InviteType.OnlyLeaderCanInvite && !plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leader can invite new people.");
                return;
            }

            string targetPlrName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
            
            TSPlayer? targetPLr = null;
            foreach(TSPlayer p in TShock.Players) {
                if (p != null && p.Active) {
                    if (p.Name.Equals(targetPlrName)) {
                        targetPLr = p;
                        break;
                    }
                    else if (p.Name.StartsWith(targetPlrName)) {
                        targetPLr = p;
                    }
                }
            }

            if (targetPLr == null) {
                plr.SendErrorMessage("Couldn't the the player.");
                return;
            }

            if (!Invitations.TryAdd(targetPLr.Name, plrFaction)) {
                if (Invitations[targetPLr.Name].Name.Equals(plrFaction.Name)) {
                    plr.SendErrorMessage("This player already has a pending invitation from your faction.");
                    return;
                }
                Invitations[targetPLr.Name] = plrFaction;
            }

            targetPLr.SendInfoMessage($"{plr.Name} has invited you to {plrFaction.Name}. Type \"/faction accept\" to join. Do nothing if you don't want to join.");
            plr.SendSuccessMessage($"You've successfully invited {targetPLr.Name} to your faction.");
        }
        
        private static void InviteTypeCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (args.Parameters.Count < 2) {    // if no args were gşven for the sub-cmd
                string inviteType = "open.";
                switch (plrFaction.InviteType) {
                    case InviteType.EveryoneCanInvite: 
                        inviteType = "invite only. (Any member can invite)";
                        break;
                    case InviteType.OnlyLeaderCanInvite:
                        inviteType = "invite only. (Only the leader can invite)";
                        break;
                }

                plr.SendInfoMessage($"Your faction is {inviteType}");
                return;
            }

            if (!plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leader can change invite type of the faction.");
                return;
            }

            // if any args were given for sub-cmd
            if (!int.TryParse(args.Parameters[1], out int newType) || newType < 1 || newType > 3) {
                plr.SendErrorMessage("Wrong command usage. (/faction invitetype [1/2/3])\n1: Open, 2: Members can invite, 3: Only leader can invite\neg.: /faction invitetype 2");
                return;
            }

            plrFaction.InviteType = (InviteType)(newType - 1);
            SFactions.DbManager.SaveFaction(plrFaction);
            plr.SendSuccessMessage($"You've successfully changed you faction's invite type to {plrFaction.InviteType}");
        }

        private static void RegionCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leader can set or delete the faction region.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You did not specify \"set\" or \"del\"");
                return;
            }
            

            switch (args.Parameters[1]) {
                case "set":
                    if (plrFaction.Region != null) {
                        plr.SendErrorMessage("You need to delete the old region before setting new one. (Do \"/faction region del\" to delete old region.)");
                        return;
                    }

                    if (plr.CurrentRegion == null) {
                        plr.SendErrorMessage("You're not in a protected region.");
                        return;
                    }

                    if (!plr.CurrentRegion.Owner.Equals(plr.Name)) {
                        plr.SendErrorMessage("You're not the owner of this region.");
                        return;
                    }

                    RegionManager.SetRegion(plr);
                    RegionManager.AddAllMembers(SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]]);
                    plr.SendSuccessMessage("Successfully set region.");
                    return;
                case "del":
                    RegionManager.DelAllMembers(plrFaction);
                    RegionManager.DelRegion(plr);
                    plr.SendSuccessMessage("Successfully deleted the region.");
                    return;
                default:
                    plr.SendErrorMessage("Invalid region subcommand. (Please use either \"set\" or \"del\")");
                    return;
            }
        }

        private static void AbilityCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leaders can change the faction ability.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("Please specify an ability. (healing, vampire, sand, adrenaline, witch, marthymr, randomtp, eol, twilight, harvest)");
                return;
            }

            AbilityType newType;
            switch (args.Parameters[1].ToLower()) {
                case "healing":
                    newType = AbilityType.DryadsRingOfHealing; break;
                case "vampire":
                    newType = AbilityType.RingOfDracula; break;
                case "sand":
                    newType = AbilityType.SandFrames; break;
                case "adrenaline":
                    newType = AbilityType.Adrenaline; break;
                case "witch":
                    newType = AbilityType.Witch; break;
                case "marthymr":
                    newType = AbilityType.Marthymr; break;
                case "randomtp":
                    newType = AbilityType.RandomTeleport; break;
                case "eol":
                    newType = AbilityType.EmpressOfLight; break;
                case "twilight":
                    newType = AbilityType.Twilight; break;
                case "harvest":
                    newType = AbilityType.Harvest; break;
                default:
                    plr.SendErrorMessage("Invalid ability type. Valid types are healing, vampire, sand, adrenaline, witch, marthymr, randomtp, eol, twilight, harvest"); return;
            }

            SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.Index]].Ability = newType;
            SFactions.DbManager.SaveFaction(SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.Index]]);
            plr.SendSuccessMessage($"Your faction's ability is now \"{args.Parameters[1]}\".");
        }

        private static void LeaveCmd(CommandArgs args) {
            if (!SFactions.OnlineMembers.ContainsKey((byte)args.Player.Index)) {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.Index]];
            RegionManager.DelMember(args.Player);
            SFactions.OnlineMembers.Remove((byte)args.Player.Index);
            SFactions.DbManager.DeleteMember(args.Player.Name);

            args.Player.SendSuccessMessage("You've left your faction.");

            if (args.Player.Name.Equals(plrFaction.Leader)) {
                plrFaction.Leader = null;
                SFactions.DbManager.SaveFaction(plrFaction);
                SFactions.OnlineFactions[plrFaction.Id].Leader = null;
                

                // check if anyone else is in the same faction and online
                foreach (int id in SFactions.OnlineMembers.Values) {
                    if (id == plrFaction.Id) {
                        return;
                    }
                }
                SFactions.OnlineFactions.Remove(plrFaction.Id);  // if no other member is online, remove the faction from onlineFactions.
            }
        }

        private static void JoinCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            string factionName;

            if (SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You need to leave your current faction to join another one.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }

            factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (!SFactions.DbManager.DoesFactionExist(factionName)) {
                plr.SendErrorMessage($"There is no faction called {factionName}.");
                return;
            }
            
            Faction newFaction = SFactions.DbManager.GetFaction(factionName);

            if (newFaction.InviteType != InviteType.Open) {
                plr.SendErrorMessage($"{newFaction.Name} is an invite only faction.");
                return;
            }

            SFactions.OnlineMembers.Add((byte)plr.Index, newFaction.Id);
            SFactions.DbManager.InsertMember(plr.Name, newFaction.Id);

            if (!SFactions.OnlineFactions.ContainsKey(newFaction.Id)) {
                SFactions.OnlineFactions.Add(newFaction.Id, newFaction);
            }
            RegionManager.AddMember(plr);
            plr.SendSuccessMessage($"You've joined {newFaction.Name}.");
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

            if (SFactions.OnlineMembers.ContainsKey((byte)args.Player.Index)) {
                plr.SendErrorMessage("You need to leave your current faction first to create new.\n" +
                    "If you want to leave your current faction do '/faction leave'");
                return;
            }

            if (SFactions.DbManager.DoesFactionExist(factionName)) {
                plr.SendErrorMessage("A faction with this name already exists.");
                return;
            }

            SFactions.DbManager.InsertFaction(plr.Name, factionName);
            Faction newFaction = SFactions.DbManager.GetFaction(factionName);
            SFactions.DbManager.InsertMember(plr.Name, newFaction.Id);
            SFactions.OnlineMembers.Add((byte)plr.Index, newFaction.Id);
            SFactions.OnlineFactions.Add(newFaction.Id, newFaction);
            args.Player.SendSuccessMessage($"You've created {factionName}");
            
        }

        private static void RenameCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leaders can change the faction name.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }
            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (SFactions.DbManager.DoesFactionExist(factionName)) {
                plr.SendErrorMessage("A faction with this name already exists.");
                return;
            }

            if (factionName.Length < SFactions.Config.MinNameLength) {
                plr.SendErrorMessage($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
                return;
            }
            if (factionName.Length > SFactions.Config.MaxNameLength) {
                plr.SendErrorMessage($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
                return;
            }

            plrFaction.Name = factionName;
            SFactions.OnlineFactions[plrFaction.Id].Name = plrFaction.Name;
            SFactions.DbManager.SaveFaction(plrFaction);
            plr.SendSuccessMessage($"Successfully changed faction name to \"{factionName}\"");
        }

        private static void LeadCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if(!SFactions.OnlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (plrFaction.Leader == null) {
                plrFaction.Leader = plr.Name;
                SFactions.DbManager.SaveFaction(plrFaction);
                SFactions.OnlineFactions[plrFaction.Id].Leader = plrFaction.Leader;
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
                + "\nability: Changes faction's ability. (usage: /faction ability <ability name>)"
                + "\nregion: Claims a protected region as faction region. (usage: /region <set/del>) (You need to be inside a protected region.)"
                );
        }
    }
}
