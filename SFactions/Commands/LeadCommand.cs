using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class LeadCommand : AbstractCommand
    {

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning disable CS8618

        protected override void Function(CommandArgs args)
        {
            _plrFaction.Leader = _plr.Name;
            SFactions.DbManager.SaveFaction(_plrFaction);
            SFactions.OnlineFactions[_plrFaction.Id].Leader = _plrFaction.Leader;
            _plr.SendSuccessMessage("You're the leader of your faction now.");
        }

        protected override bool TryParseCommands(CommandArgs args)
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