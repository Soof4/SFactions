using SFactions.Database;
using SFactions.Exceptions;
using TShockAPI;

namespace SFactions.Commands
{
    public class LeadCommand : AbstractCommand
    {
        public override string HelpText => "Used for becoming the leader of your faction if leader quits the faction.";
        public override string SyntaxHelp => "/faction lead";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plrFaction.Leader = _plr.Name;
            _ = SFactions.DbManager.SaveFactionAsync(_plrFaction);
            _plr.SendSuccessMessage("You're the leader of your faction now.");
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);

            if (_plrFaction.Leader != null)
            {
                throw new GenericCommandException($"{_plrFaction.Leader} is your faction's leader already.");
            }
        }
    }
}