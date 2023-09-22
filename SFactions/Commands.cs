using IL.Terraria.GameContent;
using TShockAPI;
using SFactions.Database;
using System.Net.Http.Headers;
using Abilities;
using NuGet.Protocol.Plugins;
using Terraria;
using Terraria.Localization;

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
                case "ability":
                    AbilityCmd(args); return;
                case "region":
                    RegionCmd(args); return;
                    /*
                case "points":
                    PointsCmd(args); return;
                case "quest":
                    QuestCmd(args); return;
                    */
                default:
                    player.SendErrorMessage("Subcommand could not be found."); return;
            }
        }
        /*
        private static async void QuestCmd(CommandArgs args) {
            TSPlayer plr = TShock.Players[args.Player.Index];
            if (!SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]];

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("No sub-command for \"quest\" command were given. (/faction quest check/deliver/new/base)");
                return;
            }

            int[] quest;
            try {
                quest = SFactions.dbManager.GetQuest(plrFaction.Id);
            }
            catch (NullReferenceException) {
                int itemId = SFactions.Config.Quests.Keys.ElementAt(WorldGen.genRand.Next(SFactions.Config.Quests.Keys.Count));
                int amount = SFactions.Config.Quests[itemId];
                SFactions.dbManager.InsertQuest(plrFaction.Id, itemId, amount);
                quest = SFactions.dbManager.GetQuest(plrFaction.Id);
            }

            switch (args.Parameters[1]) {
                case "check":
                    plr.SendInfoMessage($"Collect and deliver {quest[1]} {TShock.Utils.GetItemById(quest[0]).Name} ([i:{quest[0]}]).");
                    break;
                case "deliver":
                    if (plr.SelectedItem.netID == quest[0]) {
                        int transferAmount = plr.SelectedItem.stack;

                        if (transferAmount <= quest[1]) {
                            plr.SelectedItem.stack = 0;
                            quest[1] -= transferAmount;
                        }
                        else {
                            plr.SelectedItem.stack -= quest[1];
                            quest[1] = 0;
                        }
                        
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, plr.Index, -1, NetworkText.FromLiteral(plr.SelectedItem.Name), plr.Index, plr.TPlayer.selectedItem);
                        
                        SFactions.dbManager.SaveQuest(plrFaction.Id, quest[0], quest[1]);
                        plr.SendSuccessMessage("Delivered the items.");

                        if (quest[1] == 0) {    // quest has been completed
                            int preLevel = PointManager.GetLevelByPoint(plrFaction.Point);

                            plrFaction.Point++;
                            SFactions.dbManager.SaveFaction(plrFaction);
                            TSPlayer.All.SendMessage($"[i:903] {plrFaction.Name} completed a quest! (+1 Point)", 134, 255, 142);

                            int postLevel = PointManager.GetLevelByPoint(plrFaction.Point);

                            if (preLevel < postLevel) {    // send an announcement if leveled up
                                TSPlayer.All.SendMessage($"[i:4601] {plrFaction.Name} leveled up! (Level: {postLevel})", 134, 255, 142);
                            }

                            // create and save a new quest
                            int newItemId = SFactions.Config.Quests.Keys.ElementAt(WorldGen.genRand.Next(SFactions.Config.Quests.Keys.Count));
                            int newAmount = SFactions.Config.Quests[newItemId];
                            SFactions.dbManager.SaveQuest(plrFaction.Id, newItemId, newAmount);
                        }
                    }
                    else {
                        plr.SendErrorMessage("The item you're holding doesn't match with the quest item.");
                    }
                    break;
                case "new":
                    if (!plr.Name.Equals(plrFaction.Leader)) {
                        plr.SendErrorMessage("Only leaders can request a new quest.");
                        break;
                    }

                    if (!PointManager.QuestChangeTimes.ContainsKey(plrFaction.Id)) {
                        PointManager.QuestChangeTimes.Add(plrFaction.Id, DateTime.UtcNow);
                    }
                    else if ((DateTime.UtcNow - PointManager.QuestChangeTimes[plrFaction.Id]).TotalHours < 12) {
                        plr.SendErrorMessage("You need to wait 12 hours to change the quest again.");
                    }

                    int itemId = SFactions.Config.Quests.Keys.ElementAt(WorldGen.genRand.Next(SFactions.Config.Quests.Keys.Count));
                    int amount = SFactions.Config.Quests[itemId];
                    SFactions.dbManager.SaveQuest(plrFaction.Id, itemId, amount);
                    quest = SFactions.dbManager.GetQuest(plrFaction.Id);
                    plr.SendInfoMessage($"Collect and deliver {quest[1]} {TShock.Utils.GetItemById(quest[0]).Name} ([i:{quest[0]}]).");

                    break;
                case "base":
                    if (await PointManager.CheckForFactionBase(plr)) {
                        if (plrFaction.baseQuestComplete) {
                            plr.SendErrorMessage("Your faction has already completed the base building quest.");
                            break;
                        }
                        plrFaction.baseQuestComplete = true;
                        plrFaction.Point += 3;
                        SFactions.dbManager.SaveFaction(plrFaction);
                        TSPlayer.All.SendMessage($"[i:903] {plrFaction.Name} completed the base quest! (+3 Points)", 134, 255, 142);
                    }
                    break;
                default:
                    plr.SendErrorMessage("Invalid sub-command for \"quest\" command were given. (/faction quest check/deliver/new/base)");
                    break;
            }
        }
        
        
        private static void PointsCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]];
            plr.SendInfoMessage($"Your faction has {plrFaction.Point} points. (Level: {PointManager.GetLevelByPoint(plrFaction.Point)})");
        }
        */
        private static void RegionCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leaders can set or delete the faction region.");
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
                    RegionManager.AddAllMembers(SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]]);
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
            if (!SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]];

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

            SFactions.onlineFactions[SFactions.onlineMembers[(byte)args.Player.Index]].Ability = newType;
            SFactions.dbManager.SaveFaction(SFactions.onlineFactions[SFactions.onlineMembers[(byte)args.Player.Index]]);
            plr.SendSuccessMessage($"Your faction's ability is now \"{args.Parameters[1]}\".");
        }

        private static void LeaveCmd(CommandArgs args) {
            if (!SFactions.onlineMembers.ContainsKey((byte)args.Player.Index)) {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)args.Player.Index]];
            RegionManager.DelMember(args.Player);
            SFactions.onlineMembers.Remove((byte)args.Player.Index);
            SFactions.dbManager.DeleteMember(args.Player.Name);

            args.Player.SendSuccessMessage("You've left your faction.");

            if (args.Player.Name.Equals(plrFaction.Leader)) {
                plrFaction.Leader = null;
                SFactions.dbManager.SaveFaction(plrFaction);
                SFactions.onlineFactions[plrFaction.Id].Leader = null;
                

                // check if anyone else is in the same faction and online
                foreach (int id in SFactions.onlineMembers.Values) {
                    if (id == plrFaction.Id) {
                        return;
                    }
                }
                SFactions.onlineFactions.Remove(plrFaction.Id);  // if no other member is online, remove the faction from onlineFactions.
            }
        }

        private static void JoinCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            string factionName;

            if (SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You need to leave your current faction first to join another one.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }
            factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (!SFactions.dbManager.DoesFactionExist(factionName)) {
                plr.SendErrorMessage($"There is no faction called {factionName}.");
                return;
            }
            
            Faction newFaction = SFactions.dbManager.GetFaction(factionName);
            SFactions.onlineMembers.Add((byte)plr.Index, newFaction.Id);
            SFactions.dbManager.InsertMember(plr.Name, newFaction.Id);

            if (!SFactions.onlineFactions.ContainsKey(newFaction.Id)) {
                SFactions.onlineFactions.Add(newFaction.Id, newFaction);
            }
            RegionManager.AddMember(plr);
            plr.SendSuccessMessage($"You've joined {newFaction.Name}.");
            /*
            int memberCount = SFactions.dbManager.GetAllMembers(newFaction.Id).Count;

            if (memberCount > newFaction.highestMemberCount) {
                int preLevel = PointManager.GetLevelByPoint(newFaction.Point);

                newFaction.highestMemberCount = memberCount;
                if (memberCount % 5 == 0) {
                    newFaction.Point++;
                    TSPlayer.All.SendMessage($"[i:903] {newFaction.Name} has reached {memberCount}! (+1 Point)", 134, 255, 142);
                }

                int postLevel = PointManager.GetLevelByPoint(newFaction.Point);

                if (preLevel < postLevel) {    // send an announcement if leveled up
                    TSPlayer.All.SendMessage($"[i:4601] {newFaction.Name} has leveled up! (Level: {postLevel})", 134, 255, 142);
                }

            }
            */
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

            if (SFactions.onlineMembers.ContainsKey((byte)args.Player.Index)) {
                plr.SendErrorMessage("You need to leave your current faction first to create new.\n" +
                    "If you want to leave your current faction do '/faction leave'");
                return;
            }

            if (SFactions.dbManager.DoesFactionExist(factionName)) {
                plr.SendErrorMessage("A faction with this name already exists.");
                return;
            }

            SFactions.dbManager.InsertFaction(plr.Name, factionName);
            Faction newFaction = SFactions.dbManager.GetFaction(factionName);
            SFactions.dbManager.InsertMember(plr.Name, newFaction.Id);
            SFactions.onlineMembers.Add((byte)plr.Index, newFaction.Id);
            SFactions.onlineFactions.Add(newFaction.Id, newFaction);
            args.Player.SendSuccessMessage($"You've created {factionName}");
            
        }

        private static void RenameCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if (!SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader)) {
                plr.SendErrorMessage("Only leaders can change the faction name.");
                return;
            }

            if (args.Parameters.Count < 2) {
                plr.SendErrorMessage("You need to specify a the faction name.");
                return;
            }
            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (SFactions.dbManager.DoesFactionExist(factionName)) {
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
            SFactions.onlineFactions[plrFaction.Id].Name = plrFaction.Name;
            SFactions.dbManager.SaveFaction(plrFaction);
            plr.SendSuccessMessage($"Successfully changed faction name to \"{factionName}\"");
        }

        private static void LeadCmd(CommandArgs args) {
            TSPlayer plr = args.Player;
            if(!SFactions.onlineMembers.ContainsKey((byte)plr.Index)) {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]];

            if (plrFaction.Leader == null) {
                plrFaction.Leader = plr.Name;
                SFactions.dbManager.SaveFaction(plrFaction);
                SFactions.onlineFactions[plrFaction.Id].Leader = plrFaction.Leader;
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
                /*
                + "\npoints: Shows how many points your faction has."
                + "\nquest: Quest commands. (/faction quest check/deliver/new/base)"
                */
                );
        }
    }
}
