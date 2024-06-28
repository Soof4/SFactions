using SFactions.Database;
using TerrariaApi.Server;
using TShockAPI;

namespace SFactions.Commands
{
    public class WarCommand : AbstractCommand
    {
        public override string HelpText => "Invites, accepts or declines war invitations.";
        public override string SyntaxHelp => "/faction war <invite / accept / decline> [faction name]";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private SubCommand _subCommand;
        private static List<(Faction, Faction)> _warInvitations = new();
        public static War? ActiveWar = null;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _subCommand.Invoke(args);
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (!OnlineFactions.IsPlayerInAnyFaction(_plr))
            {
                _plr.SendErrorMessage("You're not in a faction.");
                return false;
            }

            _plrFaction = OnlineFactions.GetFaction(_plr);

            if (_plr.Name != _plrFaction.Leader)
            {
                _plr.SendErrorMessage("Only leaders can start, accept or decline a war.");
                return false;
            }

            if (args.Parameters.Count < 2)
            {
                _plr.SendErrorMessage("You need to specify a <invite/accept/decline>");
                return false;
            }

            switch (args.Parameters[1].ToLower())
            {
                case "invite":
                    _subCommand = Invite;
                    break;
                case "accept":
                    _subCommand = Accept;
                    break;
                case "decline":
                    _subCommand = Decline;
                    break;
                default:
                    _plr.SendErrorMessage("Invalid sub-command.");
                    return false;
            }

            return true;
        }

        private void Invite(CommandArgs args)
        {
            string enemyFactionName = string.Join(' ', args.Parameters.GetRange(2, args.Parameters.Count - 2));

            Faction? enemyFaction = OnlineFactions.FindFaction(enemyFactionName);

            if (enemyFaction == null)
            {
                _plr.SendErrorMessage("A faction with specified name couldn't be found online.");
                return;
            }

            TSPlayer? enemyLeader = OnlineFactions.GetLeader(enemyFaction);

            if (enemyLeader == null)
            {
                _plr.SendErrorMessage("Enemy faction's leader is not online.");
                return;
            }

            if (ActiveWar != null)
            {
                _plr.SendErrorMessage("There is another war ongoing right now. Please wait till it ends.");
                return;
            }

            foreach (var inv in _warInvitations)
            {
                if (inv.Item2.Id == enemyFaction.Id)
                {
                    _plr.SendErrorMessage("There is already a pending invitation to this faction.");
                    return;
                }
            }

            _warInvitations.Add((_plrFaction, enemyFaction));
            enemyLeader.SendInfoMessage($"{_plr.Name} has invited your faction to a war with {_plrFaction.Name}.\n" +
                                        "Do [c/ffffff:/faction war accept] to accept, [c/ffffff:/faction war decline] to decline."
                                        );
        }

        private void Accept(CommandArgs args)
        {
            foreach (var inv in _warInvitations)
            {
                if (inv.Item2.Id == _plrFaction.Id)
                {
                    _warInvitations.Remove(inv);

                    if (ActiveWar != null)
                    {
                        _plr.SendErrorMessage("There is another war ongoing right now. Please wait till it ends.");
                        return;
                    }

                    ActiveWar = new War(inv.Item1, inv.Item2);
                    ActiveWar.Start();

                    _warInvitations.Remove(inv);
                    break;
                }
            }
        }

        private void Decline(CommandArgs args)
        {
            foreach (var inv in _warInvitations)
            {
                if (inv.Item2.Id == _plrFaction.Id)
                {
                    List<TSPlayer> plrs = TSPlayer.FindByNameOrID(inv.Item1.Leader);
                    if (plrs.Count != 0)
                    {
                        plrs[0].SendErrorMessage($"{_plr.Name} declined your war invitation.");
                    }

                    _plr.SendSuccessMessage($"You've declined the war invitation.");
                    _warInvitations.Remove(inv);
                }
            }
        }
    }
}