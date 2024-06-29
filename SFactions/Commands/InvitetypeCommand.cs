using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class InvitetypeCommand : AbstractCommand
    {
        public override string HelpText => "Shows or changes your faction's invite type.";
        public override string SyntaxHelp => "/faction invitetype [open / inviteonly / closed]";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private SubCommand _subCommand;

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

            _subCommand = args.Parameters.Count < 2 ? Get : Set;

            return true;
        }

        private void Get(CommandArgs args)
        {
            _plr.SendInfoMessage($"Your faction is {Utils.ToTitleCase(_plrFaction.InviteType.ToString()).ToLower()}.");
        }

        private void Set(CommandArgs args)
        {
            if (!_plr.Name.Equals(_plrFaction.Leader))
            {
                _plr.SendErrorMessage("Only leader can change invite type of the faction.");
                return;
            }

            switch (args.Parameters[1].ToLower())
            {
                case "open":
                    _plrFaction.InviteType = InviteType.Open;
                    break;
                case "inviteonly":
                case "invite":
                    _plrFaction.InviteType = InviteType.InviteOnly;
                    break;
                case "closed":
                    _plrFaction.InviteType = InviteType.Closed;
                    break;
                default:
                    _plr.SendErrorMessage("Invalid invite type were given. Valid types are open, inviteonly, closed.");
                    return;
            }

            SFactions.DbManager.SaveFaction(_plrFaction);
            _plr.SendSuccessMessage($"You've successfully changed you faction's invite type to "
                                    + Utils.ToTitleCase(_plrFaction.InviteType.ToString()).ToLower() + ".");
        }
    }
}