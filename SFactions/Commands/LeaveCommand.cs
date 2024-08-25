using Abilities;
using SFactions.Database;
using TShockAPI;
using Abilities.Enums;
using Abilities.Abilities;

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
            FactionService.RemoveMember(_plr);
            _ = SFactions.DbManager.DeleteMemberAsync(args.Player.Name);

            args.Player.SendSuccessMessage("You've left your faction.");

            if (args.Player.Name.Equals(_plrFaction.Leader))
            {
                _plrFaction.Leader = null;
                _ = SFactions.DbManager.SaveFactionAsync(_plrFaction);

                // Check if anyone else is in the same faction and online, if not then make it offline
                if (!FactionService.IsAnyoneOnline(_plrFaction))
                {
                    FactionService.RemoveFaction(_plrFaction);
                }
            }
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);
        }
    }
}