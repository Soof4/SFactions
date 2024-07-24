using SFactions.Database;
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

        protected override void Function(CommandArgs args)
        {
            FactionService.AddMember(_plr, _newFaction);
            SFactions.DbManager.InsertMember(_plr.Name, _newFaction.Id);

            if (!FactionService.IsFactionOnline(_newFaction))
            {
                FactionService.AddFaction(_newFaction);
            }

            RegionManager.AddMember(_plr);
            _plr.SendSuccessMessage($"You've joined {_newFaction.Name}.");
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            CommandParser.IsPlayerInAnyFaction(args);
            CommandParser.IsMissingArgument(args, 1, "You need to specfiy a faction name.");

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));


            if (!SFactions.DbManager.DoesFactionExist(_factionName))
            {
                throw new GenericCommandException($"There is no faction called {_factionName}.");
            }

            _newFaction = SFactions.DbManager.GetFaction(_factionName);

            if (_newFaction.InviteType != InviteType.Open)
            {
                throw new GenericCommandException($"{_newFaction.Name} is an invite only faction.");
            }
        }
    }
}