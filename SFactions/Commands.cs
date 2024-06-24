using TShockAPI;
using SFactions.Database;
using Abilities;
using TShockAPI.Configuration;
using TerrariaApi.Server;
using Org.BouncyCastle.Tsp;

namespace SFactions
{
    public static class Commands
    {
        private static Dictionary<string, Faction> Invitations = new();
        private static List<(Faction, Faction)> _warInvitations = new();
        public static War? ActiveWar = null;
        public static void FactionCmd(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (args.Parameters.Count < 1)
            {
                player.SendErrorMessage("You need to specify a subcommand. Do '/faction help' to see all subcommands.");
                return;
            }

            string subcmd = args.Parameters[0];

            switch (subcmd)
            {
                case "create":
                    new CreateCommand().Execute(args); return;
                case "join":
                    new JoinCommand().Execute(args); return;
                case "leave":
                    new LeaveCommand().Execute(args); return;
                case "rename":
                    new RenameCommand().Execute(args); return;
                case "lead":
                    new LeadCommand().Execute(args); return;
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
                case "base":
                    BaseCmd(args); return;
                case "war":
                    WarCmd(args); return;
                default:
                    HelpCmd(args); return;
            }
        }

        private static void AcceptCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You need to leave your current faction to join another.");
                return;
            }

            if (!Invitations.TryGetValue(plr.Name, out Faction? newFaction) || newFaction == null)
            {
                plr.SendErrorMessage("Couldn't find a pending invitation.");
                return;
            }

            // Add player to the faction
            SFactions.OnlineMembers.Add((byte)plr.Index, newFaction.Id);
            SFactions.DbManager.InsertMember(plr.Name, newFaction.Id);

            if (!SFactions.OnlineFactions.ContainsKey(newFaction.Id))
            {
                SFactions.OnlineFactions.Add(newFaction.Id, newFaction);
            }

            RegionManager.AddMember(plr);
            plr.SendSuccessMessage($"You've joined {newFaction.Name}.");

        }

        private static void InfoCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (args.Parameters.Count < 2)
            {
                plr.SendErrorMessage("Please specify a faction name.");
                return;
            }

            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (!SFactions.DbManager.DoesFactionExist(factionName))
            {
                plr.SendErrorMessage($"There is no faction called {factionName}.");
                return;
            }

            Faction faction = SFactions.DbManager.GetFaction(factionName);
            plr.SendInfoMessage($"Faction ID: {faction.Id}\n" +
                                $"Faction Name: {faction.Name}\n" +
                                $"Faction Leader: {faction.Leader}\n" +
                                $"Faction Ability: {faction.Ability.GetType().Name}\n" +
                                $"Has a Region: {faction.Region != null}\n" +
                                $"Invite Type: {faction.InviteType}"
                                );
        }

        private static void InviteCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (plrFaction.InviteType == InviteType.OnlyLeaderCanInvite && !plr.Name.Equals(plrFaction.Leader))
            {
                plr.SendErrorMessage("Only leader can invite new people.");
                return;
            }

            string targetPlrName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));

            TSPlayer? targetPLr = null;
            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active)
                {
                    if (p.Name.Equals(targetPlrName))
                    {
                        targetPLr = p;
                        break;
                    }
                    else if (p.Name.StartsWith(targetPlrName))
                    {
                        targetPLr = p;
                    }
                }
            }

            if (targetPLr == null)
            {
                plr.SendErrorMessage("Couldn't find the player.");
                return;
            }

            if (!Invitations.TryAdd(targetPLr.Name, plrFaction))
            {
                if (Invitations[targetPLr.Name].Name.Equals(plrFaction.Name))
                {
                    plr.SendErrorMessage("This player already has a pending invitation from your faction.");
                    return;
                }
                Invitations[targetPLr.Name] = plrFaction;
            }

            targetPLr.SendInfoMessage($"{plr.Name} has invited you to {plrFaction.Name}. Type \"/faction accept\" to join. Do nothing if you don't want to join.");
            plr.SendSuccessMessage($"You've successfully invited {targetPLr.Name} to your faction.");
        }

        private static void InviteTypeCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (args.Parameters.Count < 2)
            {    // if no args were gşven for the sub-cmd
                string inviteType = "open.";
                switch (plrFaction.InviteType)
                {
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

            if (!plr.Name.Equals(plrFaction.Leader))
            {
                plr.SendErrorMessage("Only leader can change invite type of the faction.");
                return;
            }

            // if any args were given for sub-cmd
            if (!int.TryParse(args.Parameters[1], out int newType) || newType < 1 || newType > 3)
            {
                plr.SendErrorMessage("Wrong command usage. (/faction invitetype [1/2/3])\n1: Open, 2: Members can invite, 3: Only leader can invite\neg.: /faction invitetype 2");
                return;
            }

            plrFaction.InviteType = (InviteType)(newType - 1);
            SFactions.DbManager.SaveFaction(plrFaction);
            plr.SendSuccessMessage($"You've successfully changed you faction's invite type to {plrFaction.InviteType}");
        }

        private static void RegionCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader))
            {
                plr.SendErrorMessage("Only leader can set or delete the faction region.");
                return;
            }

            if (args.Parameters.Count < 2)
            {
                plr.SendErrorMessage("You did not specify \"set\" or \"del\"");
                return;
            }


            switch (args.Parameters[1])
            {
                case "set":
                    if (plrFaction.Region != null)
                    {
                        plr.SendErrorMessage("You need to delete the old region before setting new one. (Do \"/faction region del\" to delete old region.)");
                        return;
                    }

                    if (plr.CurrentRegion == null)
                    {
                        plr.SendErrorMessage("You're not in a protected region.");
                        return;
                    }

                    if (!plr.CurrentRegion.Owner.Equals(plr.Name))
                    {
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

        private static void AbilityCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader))
            {
                plr.SendErrorMessage("Only leaders can change the faction ability.");
                return;
            }

            if (args.Parameters.Count < 2)
            {
                plr.SendErrorMessage("Invalid ability type. Valid types are:\n" +
                                     string.Join(", ", SFactions.Config.EnabledAbilities));
                return;
            }

            if (!Utils.TryGetAbilityTypeFromString(args.Parameters[1].ToLower(), out AbilityType newType))
            {
                plr.SendErrorMessage("Invalid ability type. Valid types are:\n" +
                                     string.Join(", ", SFactions.Config.EnabledAbilities));
                return;
            }

            if (!SFactions.Config.EnabledAbilities.Contains(args.Parameters[1].ToLower()))
            {
                plr.SendErrorMessage("Invalid ability type. Valid types are:\n" +
                                     string.Join(", ", SFactions.Config.EnabledAbilities));
                return;
            }

            plrFaction.AbilityType = newType;
            SFactions.OnlineFactions[plrFaction.Id] = new Faction(plrFaction.Id, plrFaction.Name, plrFaction.Leader,
                                                                  plrFaction.AbilityType, plrFaction.Region, DateTime.UtcNow, plrFaction.InviteType);
            SFactions.DbManager.SaveFaction(SFactions.OnlineFactions[plrFaction.Id]);

            plr.SendSuccessMessage($"Your faction's ability is now \"{args.Parameters[1]}\".");
        }

        private static void LeaveCmd(CommandArgs args)
        {
            /*
            if (!SFactions.OnlineMembers.ContainsKey((byte)args.Player.Index))
            {
                args.Player.SendErrorMessage("You're not in a faction.");
                return;
            }

            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.Index]];

            if (plrFaction.AbilityType == AbilityType.TheBound)
            {
                TheBound.Pairs.Remove(args.Player);
            }

            RegionManager.DelMember(args.Player);
            SFactions.OnlineMembers.Remove((byte)args.Player.Index);
            SFactions.DbManager.DeleteMember(args.Player.Name);

            args.Player.SendSuccessMessage("You've left your faction.");

            if (args.Player.Name.Equals(plrFaction.Leader))
            {
                plrFaction.Leader = null;
                SFactions.DbManager.SaveFaction(plrFaction);
                SFactions.OnlineFactions[plrFaction.Id].Leader = null;


                // check if anyone else is in the same faction and online
                foreach (int id in SFactions.OnlineMembers.Values)
                {
                    if (id == plrFaction.Id)
                    {
                        return;
                    }
                }
                SFactions.OnlineFactions.Remove(plrFaction.Id);  // if no other member is online, remove the faction from onlineFactions.
            }
            */
        }

        private static void RenameCmd(CommandArgs args)
        {
            /*
            TSPlayer plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader))
            {
                plr.SendErrorMessage("Only leaders can change the faction name.");
                return;
            }

            if (args.Parameters.Count < 2)
            {
                plr.SendErrorMessage("You need to specify a faction name.");
                return;
            }
            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (SFactions.DbManager.DoesFactionExist(factionName))
            {
                plr.SendErrorMessage("A faction with this name already exists.");
                return;
            }

            if (factionName.Length < SFactions.Config.MinNameLength)
            {
                plr.SendErrorMessage($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
                return;
            }
            if (factionName.Length > SFactions.Config.MaxNameLength)
            {
                plr.SendErrorMessage($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
                return;
            }

            plrFaction.Name = factionName;
            SFactions.OnlineFactions[plrFaction.Id].Name = plrFaction.Name;
            SFactions.DbManager.SaveFaction(plrFaction);
            plr.SendSuccessMessage($"Successfully changed faction name to \"{factionName}\"");
            */
        }

        private static void LeadCmd(CommandArgs args)
        {
            /*
            TSPlayer plr = args.Player;

            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }
            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (plrFaction.Leader == null)
            {
                plrFaction.Leader = plr.Name;
                SFactions.DbManager.SaveFaction(plrFaction);
                SFactions.OnlineFactions[plrFaction.Id].Leader = plrFaction.Leader;
                plr.SendSuccessMessage("You're the leader of your faction now.");
            }
            else
            {
                plr.SendErrorMessage($"{plrFaction.Leader} is your faction's leader already.");
            }
            */
        }

        private static void BaseCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;

            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }

            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (plrFaction.BaseX == null || plrFaction.BaseY == null)
            {
                plr.SendErrorMessage("Your faction doesn't have a base!");
                return;
            }

            plr.Teleport((float)(16 * plrFaction.BaseX), (float)(16 * plrFaction.BaseY));
        }

        private static void WarCmd(CommandArgs args)
        {
            TSPlayer plr = args.Player;

            if (!SFactions.OnlineMembers.ContainsKey((byte)plr.Index))
            {
                plr.SendErrorMessage("You're not in a faction.");
                return;
            }

            Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)plr.Index]];

            if (!plr.Name.Equals(plrFaction.Leader))
            {
                plr.SendErrorMessage("Only leaders can start, accept or decline a war.");
                return;
            }

            if (args.Parameters.Count < 2)
            {
                plr.SendErrorMessage("You need to specify a <invite/accept/decline>");
                return;
            }

            switch (args.Parameters[1].ToLower())
            {
                case "invite":
                    {
                        string enemyFactionName = string.Join(' ', args.Parameters.GetRange(2, args.Parameters.Count - 2));

                        Faction? enemyFaction = null;
                        foreach (Faction f in SFactions.OnlineFactions.Values)
                        {
                            if (f.Name == enemyFactionName)
                            {
                                enemyFaction = f;
                                break;
                            }
                            else if (f.Name.StartsWith(enemyFactionName))
                            {
                                enemyFaction = f;
                            }
                        }

                        if (enemyFaction == null)
                        {
                            plr.SendErrorMessage("A faction with specified name couldn't be found online.");
                            return;
                        }

                        TSPlayer? enemyLeader = null;
                        foreach (var kvp in SFactions.OnlineMembers)
                        {
                            if (kvp.Value == enemyFaction.Id && TShock.Players[kvp.Key].Name == enemyFaction.Leader)
                            {
                                enemyLeader = TShock.Players[kvp.Key];
                                break;
                            }
                        }

                        if (enemyLeader == null)
                        {
                            plr.SendErrorMessage("Enemy faction's leader is not online.");
                            return;
                        }

                        if (ActiveWar != null)
                        {
                            plr.SendErrorMessage("There is another war ongoing right now. Please wait till it ends.");
                            return;
                        }

                        foreach (var inv in _warInvitations)
                        {
                            if (inv.Item2.Id == enemyFaction.Id)
                            {
                                plr.SendErrorMessage("There is already a pending invitation to this faction.");
                                return;
                            }
                        }

                        _warInvitations.Add((plrFaction, enemyFaction));
                        enemyLeader.SendInfoMessage($"{plr.Name} has invited your faction to a war with {plrFaction.Name}.\n" +
                                                    "Do [c/ffffff:/faction war accept] to accept, [c/ffffff:/faction war decline] to decline.");
                        break;
                    }
                case "accept":
                    {
                        foreach (var inv in _warInvitations)
                        {
                            if (inv.Item2.Id == plrFaction.Id)
                            {
                                _warInvitations.Remove(inv);

                                if (ActiveWar != null)
                                {
                                    plr.SendErrorMessage("There is another war ongoing right now. Please wait till it ends.");
                                    return;
                                }

                                ActiveWar = new War(inv.Item1, inv.Item2);
                                ActiveWar.Start();

                                _warInvitations.Remove(inv);
                                break;
                            }
                        }
                        break;
                    }
                case "decline":
                    {
                        foreach (var inv in _warInvitations)
                        {
                            if (inv.Item2.Id == plrFaction.Id)
                            {
                                List<TSPlayer> plrs = TSPlayer.FindByNameOrID(inv.Item1.Leader);
                                if (plrs.Count != 0)
                                {
                                    plrs[0].SendErrorMessage($"{plr.Name} declined your war invitation.");
                                }

                                plr.SendSuccessMessage($"You've declined the war invitation.");
                                _warInvitations.Remove(inv);
                            }
                        }
                        break;
                    }
                default:
                    {
                        plr.SendErrorMessage("Invalid sub-command.");
                        return;
                    }
            }





        }

        private static void HelpCmd(CommandArgs args)
        {
            args.Player.SendInfoMessage("Subcommands:"
                + "\nhelp: Shows this message."
                + "\ncreate: Create a new faction (usage: /faction create <faction name>)"
                + "\njoin: Join a faction: (usage: /faction join <faction name>)"
                + "\nleave: Leave your faction"
                + "\nrename: Changes faction name, requires leadership permissions. (usage: /faction rename <name>)"
                + "\nlead: Make yourself the leader of your faction if there isn't someone else already."
                + "\nability: Changes faction's ability. (usage: /faction ability <ability name>)"
                + "\nregion: Claims a protected region as faction region. (usage: /region <set/del>) (You need to be inside a protected region.)"
                + "\ninvitetype: Shows your faction's invite type but if you're the leader you can change the invite type such as \"/f invitetype 3\""
                + "\ninvite: Invites the target player to your faction."
                + "\naccept: Accepts the last faction invitation."
                + "\ninfo: Shows the information of target faction."
                + "\nbase: Teleports you to the faction base."
                + "\nwar: Starts a war between factions. (usage: /war invite <enemy faction name>, /f war <accept/decline>)"
                );
        }
    }
}
