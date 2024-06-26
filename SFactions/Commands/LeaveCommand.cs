using Abilities;
using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class LeaveCommand : AbstractCommand
    {
        public override string HelpText => "Used for leaving your current faction.";
        public override string SyntaxHelp => "/faction leave";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            if (_plrFaction.AbilityType == AbilityType.TheBound)
            {
                TheBound.Pairs.Remove(args.Player);
            }

            RegionManager.DelMember(args.Player);
            OnlineFactions.RemoveMember(_plr);
            SFactions.DbManager.DeleteMember(args.Player.Name);

            args.Player.SendSuccessMessage("You've left your faction.");

            if (args.Player.Name.Equals(_plrFaction.Leader))
            {
                _plrFaction.Leader = null;
                SFactions.DbManager.SaveFaction(_plrFaction);

                // Check if anyone else is in the same faction and online, if not then make it offline
                if (!OnlineFactions.IsAnyoneOnline(_plrFaction))
                {
                    OnlineFactions.RemoveFaction(_plrFaction);
                }
            }
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (!OnlineFactions.IsPlayerInAnyFaction(_plr))
            {
                args.Player.SendErrorMessage("You're not in a faction.");
                return false;
            }

            _plrFaction = OnlineFactions.GetFaction(_plr);

            return true;
        }
    }
}