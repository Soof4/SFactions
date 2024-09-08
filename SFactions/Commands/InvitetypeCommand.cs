using SFactions.Database;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class InvitetypeCommand : AbstractCommand
    {
        public override string HelpText => Localization.InviteTypeCommand_HelpText;
        public override string SyntaxHelp => Localization.InviteTypeCommand_SyntaxHelp;
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

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);
            _subCommand = args.Parameters.Count < 2 ? Get : Set;
        }

        private void Get(CommandArgs args)
        {
            _plr.SendInfoMessage(string.Format(
                Localization.InviteTypeCommand_GetResult,
                _plrFaction.InviteType.ToString().ToTitleCase()
            ));
        }

        private void Set(CommandArgs args)
        {
            if (!_plr.Name.Equals(_plrFaction.Leader))
            {
                _plr.SendErrorMessage(Localization.ErrorMessage_LeaderOnly);
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
                    _plr.SendErrorMessage(Localization.InviteTypeCommand_ErrorMessage_InvalidType);
                    return;
            }

            _ = SFactions.DbManager.SaveFactionAsync(_plrFaction);

            _plr.SendSuccessMessage(
                string.Format(
                    Localization.InviteTypeCommand_SetResult,
                    _plrFaction.InviteType.ToString().ToTitleCase().ToLower()
                )
            );
        }
    }
}