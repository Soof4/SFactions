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
            OnlineFactions.AddMember(_plr, _faction);
            SFactions.DbManager.InsertMember(_plr.Name, _faction.Id);

            if (!OnlineFactions.IsFactionOnline(_faction))
            {
                OnlineFactions.AddFaction(_faction);
            }

            RegionManager.AddMember(_plr);
            _plr.SendSuccessMessage($"You've joined {_faction.Name}.");
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (OnlineFactions.IsPlayerInAnyFaction(_plr))
            {
                _plr.SendErrorMessage("You need to leave your current faction to join another.");
                return false;
            }

            if (!SFactions.Invitations.ContainsKey(_plr.Name))
            {
                _plr.SendErrorMessage("Couldn't find a pending invitation.");
                return false;
            }
            else
            {
                _faction = SFactions.Invitations[_plr.Name];
            }

            return true;
        }
    }
}