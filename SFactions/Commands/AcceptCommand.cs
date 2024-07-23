using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class AcceptCommand : AbstractCommand
    {
        public override string HelpText => "Accepts a faction invite.";
        public override string SyntaxHelp => "/faction accept";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _faction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            FactionService.AddMember(_plr, _faction);
            SFactions.DbManager.InsertMember(_plr.Name, _faction.Id);

            if (!FactionService.IsFactionOnline(_faction))
            {
                FactionService.AddFaction(_faction);
            }

            SFactions.Invitations.Remove(_plr.Name);
            RegionManager.AddMember(_plr);
            _plr.SendSuccessMessage($"You've joined {_faction.Name}.");
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (FactionService.IsPlayerInAnyFaction(_plr))
            {
                throw new CommandException("You need to leave your current faction to join another.");
            }

            if (!SFactions.Invitations.ContainsKey(_plr.Name))
            {
                throw new CommandException("Couldn't find a pending invitation.");
            }
            else
            {
                _faction = SFactions.Invitations[_plr.Name];
            }
        }
    }
}