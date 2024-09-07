using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class AcceptCommand : AbstractCommand
    {
        public override string HelpText => Localization.AcceptCommand_HelpText;
        public override string SyntaxHelp => Localization.AcceptCommand_SyntaxHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _faction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            FactionService.AddMember(_plr, _faction);
            _ = SFactions.DbManager.InsertMemberAsync(_plr.Name, _faction.Id);

            if (!FactionService.IsFactionOnline(_faction))
            {
                FactionService.AddFaction(_faction);
            }

            SFactions.Invitations.Remove(_plr.Name);
            RegionManager.AddMember(_plr);
            _plr.SendSuccessMessage(string.Format(Localization.AcceptCommand_SuccessMessage, _faction.Name));
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (FactionService.IsPlayerInAnyFaction(_plr))
            {
                throw new GenericCommandException(Localization.AcceptCommand_ErrorMessage_MustLeaveFaction);
            }

            if (!SFactions.Invitations.ContainsKey(_plr.Name))
            {
                throw new GenericCommandException(Localization.ErrorMessage_NoInviteFound);
            }
            else
            {
                _faction = SFactions.Invitations[_plr.Name];
            }
        }
    }
}