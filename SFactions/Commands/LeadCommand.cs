using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class LeadCommand : AbstractCommand
    {
        public override string HelpText => Localization.LeadCommand_HelpText;
        public override string SyntaxHelp => Localization.LeadCommand_SyntaxHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plrFaction.Leader = _plr.Name;
            _ = SFactions.DbManager.SaveFactionAsync(_plrFaction);
            _plr.SendSuccessMessage(Localization.LeadCommand_SuccessMessage);
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);

            if (_plrFaction.Leader != null)
            {
                throw new GenericCommandException(
                    string.Format(
                        Localization.LeadCommand_ErrorMessage_LeaderExists,
                        _plrFaction.Leader
                    )
                );
            }
        }
    }
}