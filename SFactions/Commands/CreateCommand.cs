using SFactions.Database;
using SFactions.Exceptions;
using TShockAPI;

namespace SFactions.Commands
{
    public class CreateCommand : AbstractCommand
    {
        public override string HelpText => "Creates a new faction.";
        public override string SyntaxHelp => "/faction create <faction name>";
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
                    throw new GenericCommandException("A faction with this name already exists.");
                }

                await SFactions.DbManager.InsertFactionAsync(_plr.Name, _factionName);
                Faction newFaction = await SFactions.DbManager.GetFactionAsync(_factionName);
                await SFactions.DbManager.InsertMemberAsync(_plr.Name, newFaction.Id);
                FactionService.AddMember(_plr, newFaction);
                FactionService.AddFaction(newFaction);
                _plr.SendSuccessMessage($"You've created {_factionName}.");
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

            if (FactionService.IsPlayerInAnyFaction(_plr))
            {
                throw new GenericCommandException("You need to leave your current faction first to create new.\n" +
                                           "If you want to leave your current faction do '/faction leave'");
            }

            CommandParser.IsMissingArgument(args, 1, "You need to specify a the faction name.");

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (_factionName.Length < SFactions.Config.MinNameLength)
            {
                throw new GenericCommandException($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
            }

            if (_factionName.Length > SFactions.Config.MaxNameLength)
            {
                throw new GenericCommandException($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
            }
        }
    }
}