using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class InfoCommand : AbstractCommand
    {
        public override string HelpText => Localization.InfoCommand_HelpText;
        public override string SyntaxHelp => Localization.InfoCommand_SyntaxHelp;
        protected override bool AllowServer => true;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _faction;
        private string _factionName;

#pragma warning restore CS8618

        protected override async void Function(CommandArgs args)
        {
            try
            {
                if (!await SFactions.DbManager.DoesFactionExistAsync(_factionName))
                {
                    throw new FactionNotFoundCommandException();
                }

                _faction = await SFactions.DbManager.GetFactionAsync(_factionName);

                _plr.SendInfoMessage(
                    string.Format(
                        Localization.InfoCommand_Result,
                        _faction.Id,
                        _faction.Name,
                        _faction.Leader,
                        _faction.Ability.GetType().Name.ToTitleCase(),
                        _faction.Region != null,
                        _faction.InviteType.ToString().ToTitleCase()
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

            CommandParser.IsMissingArgument(args, 1, Localization.ErrorMessage_MissingFactionName);

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));
        }
    }
}