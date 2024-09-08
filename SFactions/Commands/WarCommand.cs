using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class WarCommand : AbstractCommand
    {
        public override string HelpText => Localization.WarCommand_HelpText;
        public override string SyntaxHelp => Localization.WarCommand_SyntaxHelp;
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

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);

            CommandParser.IsPlayerTheLeader(_plrFaction, _plr);
            CommandParser.IsMissingArgument(args, 1, Localization.WarCommand_ErrorMessage_MissingSubCommand);

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
                    throw new GenericCommandException(Localization.WarCommand_ErrorMessage_InvalidSubCommand);
            }
        }

        private void Invite(CommandArgs args)
        {
            string enemyFactionName = string.Join(' ', args.Parameters.GetRange(2, args.Parameters.Count - 2));

            Faction? enemyFaction = FactionService.FindFaction(enemyFactionName);

            if (enemyFaction == null)
            {
                _plr.SendErrorMessage(Localization.WarCommand_ErrorMessage_FactionNotOnline);
                return;
            }

            TSPlayer? enemyLeader = FactionService.GetLeader(enemyFaction);

            if (enemyLeader == null)
            {
                _plr.SendErrorMessage(Localization.WarCommand_ErrorMessage_EnemyLeaderNotOnline);
                return;
            }

            if (enemyFaction == _plrFaction)
            {
                _plr.SendErrorMessage(Localization.WarCommand_ErrorMessage_CantAttackYourself);
                return;
            }

            if (ActiveWar != null)
            {
                _plr.SendErrorMessage(Localization.WarCommand_ErrorMessage_OngoingWar);
                return;
            }

            foreach (var inv in _warInvitations)
            {
                if (inv.Item2.Id == enemyFaction.Id)
                {
                    _plr.SendErrorMessage(Localization.WarCommand_ErrorMessage_PendingInvite);
                    return;
                }
            }

            _warInvitations.Add((_plrFaction, enemyFaction));
            enemyLeader.SendInfoMessage(
                string.Format(
                    Localization.WarCommand_NotificationMessage_GotInvite,
                    _plr.Name,
                    _plrFaction.Name
                )
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
                        _plr.SendErrorMessage(Localization.WarCommand_ErrorMessage_OngoingWar);
                        return;
                    }

                    ActiveWar = new War(inv.Item1, inv.Item2);
                    ActiveWar.Start();

                    _warInvitations.Remove(inv);
                    return;
                }
            }

            _plr.SendErrorMessage(Localization.ErrorMessage_NoInviteFound);
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
                        plrs[0].SendErrorMessage(
                            string.Format(
                                Localization.WarCommand_NotificationMessage_Declined,
                                _plr.Name
                            )
                        );
                    }

                    _plr.SendSuccessMessage(Localization.WarCommand_SuccessMessage_Declined);
                    _warInvitations.Remove(inv);
                    return;
                }
            }

            _plr.SendErrorMessage(Localization.ErrorMessage_NoInviteFound);
        }
    }
}