using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class RegionCommand : AbstractCommand
    {
        public override string HelpText => Localization.RegionCommand_HelpText;
        public override string SyntaxHelp => Localization.RegionCommand_SyntaxHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private SubCommand _subCommand;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _subCommand.Invoke(args);
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            _plrFaction = CommandParser.GetPlayerFaction(args);

            CommandParser.IsPlayerTheLeader(_plrFaction, _plr);


            CommandParser.IsMissingArgument(args, 1, Localization.RegionCommand_MissingSubCommand);

            switch (args.Parameters[1].ToLower())
            {
                case "set":
                    _subCommand = Set;
                    break;
                case "del":
                    _subCommand = Delete;
                    break;
                default:
                    throw new GenericCommandException(Localization.RegionCommand_ErrorMessage_InvalidSubCommand);
            }
        }

        private void Set(CommandArgs args)
        {
            if (_plrFaction.Region != null)
            {
                _plr.SendErrorMessage(Localization.RegionCommand_ErrorMessage_MustDeleteOldOne);
                return;
            }

            if (_plr.CurrentRegion == null)
            {
                _plr.SendErrorMessage(Localization.RegionCommand_ErrorMessage_NotInARegion);
                return;
            }

            if (_plr.CurrentRegion.Owner != _plr.Name)
            {
                _plr.SendErrorMessage(Localization.RegionCommand_ErrorMessage_NotOwner);
                return;
            }

            RegionManager.SetRegion(_plr);
            RegionManager.AddAllMembers(_plrFaction);
            _plr.SendSuccessMessage(Localization.RegionCommand_SuccessMessage_Set);
        }

        private void Delete(CommandArgs args)
        {
            if (_plrFaction.Region == null)
            {
                _plr.SendErrorMessage(Localization.RegionCommand_ErrorMessage_NoFactionRegion);
                return;
            }

            RegionManager.DelAllMembers(_plrFaction);
            RegionManager.DelRegion(_plr);
            _plr.SendSuccessMessage(Localization.RegionCommand_SuccessMessage_Del);
        }
    }
}