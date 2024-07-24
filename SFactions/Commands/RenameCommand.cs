using SFactions.Database;
using SFactions.Exceptions;
using TShockAPI;

namespace SFactions.Commands
{
    public class RenameCommand : AbstractCommand
    {
        public override string HelpText => "Renames the faction.";
        public override string SyntaxHelp => "/faction rename <new name>";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private string _factionName;

#pragma warning restore CS8618

        protected override async void Function(CommandArgs args)
        {
            try
            {
                if (await SFactions.DbManager.DoesFactionExistAsync(_factionName))
                {
                    throw new GenericCommandException("A faction with this name already exists.");
                }

                if (_factionName.Length < SFactions.Config.MinNameLength)
                {
                    throw new GenericCommandException($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
                }

                if (_factionName.Length > SFactions.Config.MaxNameLength)
                {
                    throw new GenericCommandException($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
                }

                _plrFaction.Name = _factionName;
                await SFactions.DbManager.SaveFactionAsync(_plrFaction);
                _plr.SendSuccessMessage($"Successfully changed faction name to \"{_factionName}\"");
            }
            catch (GenericCommandException e)
            {
                _plr.SendErrorMessage(e.ErrorMessage);
            }
            catch (Exception e)
            {
                _plr.SendErrorMessage("Command failed, check logs for more details.");
                TShock.Log.Error(e.ToString());
            }
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);
            CommandParser.IsPlayerTheLeader(_plrFaction, _plr);
            CommandParser.IsMissingArgument(args, 1, "You need to specify a faction name.");

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));
        }
    }
}