using SFactions.Database;
using SFactions.Exceptions;
using TShockAPI;

namespace SFactions.Commands
{
    public class JoinCommand : AbstractCommand
    {
        public override string HelpText => "Used for joining an open faction.";
        public override string SyntaxHelp => "/faction join <faction name>";
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
                    throw new GenericCommandException($"There is no faction called {_factionName}.");
                }

                _newFaction = await SFactions.DbManager.GetFactionAsync(_factionName);

                if (_newFaction.InviteType != InviteType.Open)
                {
                    throw new GenericCommandException($"{_newFaction.Name} is an invite only faction.");
                }

                FactionService.AddMember(_plr, _newFaction);
                await SFactions.DbManager.InsertMemberAsync(_plr.Name, _newFaction.Id);

                if (!FactionService.IsFactionOnline(_newFaction))
                {
                    FactionService.AddFaction(_newFaction);
                }

                RegionManager.AddMember(_plr);
                _plr.SendSuccessMessage($"You've joined {_newFaction.Name}.");
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

            CommandParser.IsPlayerNotInAnyFaction(args);
            CommandParser.IsMissingArgument(args, 1, "You need to specfiy a faction name.");

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));



        }
    }
}