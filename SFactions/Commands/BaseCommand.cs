using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class BaseCommand : AbstractCommand
    {
        public override string HelpText => Localization.BaseCommand_HelpText;
        public override string SyntaxHelp => Localization.BaseCommand_SyntaxHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plr.Teleport((float)(16 * _plrFaction.BaseX!), (float)(16 * _plrFaction.BaseY!));
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);

            if (_plrFaction.BaseX == null || _plrFaction.BaseY == null)
            {
                throw new GenericCommandException(Localization.BaseCommand_ErrorMessage_NoBase);
            }
        }
    }
}