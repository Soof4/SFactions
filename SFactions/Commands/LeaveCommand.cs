using Abilities;
using SFactions.Database;
using TShockAPI;
using Abilities.Enums;
using Abilities.Abilities;
using SFactions.i18net;

namespace SFactions.Commands
{
    public class LeaveCommand : AbstractCommand
    {
        public override string HelpText => Localization.LeaveCommand_HelpText;
        public override string SyntaxHelp => Localization.LeaveCommand_SyntaxHelp;
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

            args.Player.SendSuccessMessage(Localization.LeaveCommand_SuccessMessage);

            if (args.Player.Name.Equals(_plrFaction.Leader))
            {
                _plrFaction.Leader = null;
                _ = SFactions.DbManager.SaveFactionAsync(_plrFaction);

                // Check if anyone else is in the same faction and online, if not then make it offline
                if (!FactionService.IsAnyoneOnline(_plrFaction))
                {
                    FactionService.RemoveFaction(_plrFaction);
                }

                Task.Run(async () =>
                {
                    List<string> members = await SFactions.DbManager.GetAllMembersAsync(_plrFaction.Id);
                    if (members.Count == 0)
                    {
                        await SFactions.DbManager.DeleteFactionAsync(_plrFaction.Id);
                    }
                });
            }
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);
        }
    }
}