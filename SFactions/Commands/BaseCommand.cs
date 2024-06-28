using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class BaseCommand : AbstractCommand
    {
        public override string HelpText => "Teleports player to the faction base.";
        public override string SyntaxHelp => "/faction base";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plr.Teleport((float)(16 * _plrFaction.BaseX!), (float)(16 * _plrFaction.BaseY!));
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

            if (_plrFaction.BaseX == null || _plrFaction.BaseY == null)
            {
                _plr.SendErrorMessage("Your faction doesn't have a base!");
                return false;
            }

            return true;
        }
    }
}