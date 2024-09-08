using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class JoinCommand : AbstractCommand
    {
        public override string HelpText => Localization.JoinCommand_HelpText;
        public override string SyntaxHelp => Localization.JoinCommand_SyntaxHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private string _factionName;
        private Faction _newFaction;

#pragma warning restore CS8618

        protected override async void Function(CommandArgs args)
        {
            try
            {
                if (!await SFactions.DbManager.DoesFactionExistAsync(_factionName))
                {
                    throw new GenericCommandException(
                        string.Format(
                            Localization.ErrorMessage_FactionNotFound,
                            _factionName
                        )
                    );
                }

                _newFaction = await SFactions.DbManager.GetFactionAsync(_factionName);

                if (_newFaction.InviteType != InviteType.Open)
                {
                    throw new GenericCommandException(
                        string.Format(
                            Localization.JoinCommand_ErrorMessage_InviteOnly,
                            _newFaction.Name
                        )
                    );
                }

                FactionService.AddMember(_plr, _newFaction);
                await SFactions.DbManager.InsertMemberAsync(_plr.Name, _newFaction.Id);

                if (!FactionService.IsFactionOnline(_newFaction))
                {
                    FactionService.AddFaction(_newFaction);
                }

                RegionManager.AddMember(_plr);
                _plr.SendSuccessMessage(
                    string.Format(
                        Localization.JoinCommand_SuccessMessage,
                        _newFaction.Name
                    )
                );
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

            CommandParser.IsPlayerNotInAnyFaction(args);
            CommandParser.IsMissingArgument(args, 1, Localization.ErrorMessage_MissingFactionName);

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));
        }
    }
}