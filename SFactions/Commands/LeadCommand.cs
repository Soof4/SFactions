using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class LeadCommand : AbstractCommand
    {
        public override string HelpText => "Used for becoming the leader of your faction if leader quits the faction.";
        public override string SyntaxHelp => "/faction lead";
        
#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plrFaction.Leader = _plr.Name;
            SFactions.DbManager.SaveFaction(_plrFaction);
            SFactions.OnlineFactions[_plrFaction.Id].Leader = _plrFaction.Leader;
            _plr.SendSuccessMessage("You're the leader of your faction now.");
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (!SFactions.OnlineMembers.ContainsKey((byte)_plr.Index))
            {
                _plr.SendErrorMessage("You're not in a faction.");
                return false;
            }

            _plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)_plr.Index]];

            if (_plrFaction.Leader != null)
            {
                _plr.SendErrorMessage($"{_plrFaction.Leader} is your faction's leader already.");
                return false;
            }

            return true;
        }
    }
}