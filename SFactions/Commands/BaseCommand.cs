using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class BaseCommand : AbstractCommand
    {

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning disable CS8618

        protected override void Function(CommandArgs args)
        {
            _plr.Teleport((float)(16 * _plrFaction.BaseX!), (float)(16 * _plrFaction.BaseY!));
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

            if (_plrFaction.BaseX == null || _plrFaction.BaseY == null)
            {
                _plr.SendErrorMessage("Your faction doesn't have a base!");
                return false;
            }

            return true;
        }
    }
}