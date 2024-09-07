using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class CreateCommand : AbstractCommand
    {
        public override string HelpText => Localization.CreateCommand_HelpText;
        public override string SyntaxHelp => Localization.CreateCommand_SyntaxHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private string _factionName;

#pragma warning restore CS8618

        protected override async void Function(CommandArgs args)
        {
            try
            {
                if (await SFactions.DbManager.DoesFactionExistAsync(_factionName))
                {
                    throw new GenericCommandException(Localization.ErrorMessage_NameTaken);
                }

                await SFactions.DbManager.InsertFactionAsync(_plr.Name, _factionName);
                Faction newFaction = await SFactions.DbManager.GetFactionAsync(_factionName);
                await SFactions.DbManager.InsertMemberAsync(_plr.Name, newFaction.Id);
                FactionService.AddMember(_plr, newFaction);
                FactionService.AddFaction(newFaction);
                _plr.SendSuccessMessage(string.Format(Localization.CreateCommand_SuccessMessage, _factionName));
            }
            catch (GenericCommandException e)
            {
                _plr.SendErrorMessage(e.ErrorMessage);
            }
            catch (Exception e)
            {
                _plr.SendErrorMessage(Localization.ErrorMessage_GenericFail);
                TShock.Log.Error(e.ToString());
            }
        }
        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (FactionService.IsPlayerInAnyFaction(_plr))
            {
                throw new GenericCommandException(Localization.CreateCommand_ErrorMessage_MustLeaveFaction);
            }

            CommandParser.IsMissingArgument(args, 1, Localization.ErrorMessage_MissingFactionName);

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (_factionName.Length < SFactions.Config.MinNameLength)
            {
                throw new GenericCommandException(string.Format(Localization.ErrorMessage_FactionNameTooShort, SFactions.Config.MinNameLength));
            }

            if (_factionName.Length > SFactions.Config.MaxNameLength)
            {
                throw new GenericCommandException(string.Format(Localization.ErrorMessage_FactionNameTooLong, SFactions.Config.MaxNameLength));
            }
        }
    }
}