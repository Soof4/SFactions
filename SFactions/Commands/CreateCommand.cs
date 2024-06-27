using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class CreateCommand : AbstractCommand
    {
        public override string HelpText => "Creates a new faction.";
        public override string SyntaxHelp => "/faction create <faction name>";

#pragma warning disable CS8618

        private TSPlayer _plr;
        private string _factionName;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            SFactions.DbManager.InsertFaction(_plr.Name, _factionName);
            Faction newFaction = SFactions.DbManager.GetFaction(_factionName);
            SFactions.DbManager.InsertMember(_plr.Name, newFaction.Id);
            SFactions.OnlineMembers.Add((byte)_plr.Index, newFaction.Id);
            SFactions.OnlineFactions.Add(newFaction.Id, newFaction);
            args.Player.SendSuccessMessage($"You've created {_factionName}");
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (SFactions.OnlineMembers.ContainsKey((byte)args.Player.Index))
            {
                _plr.SendErrorMessage("You need to leave your current faction first to create new.\n" +
                                     "If you want to leave your current faction do '/faction leave'");
                return false;
            }

            if (args.Parameters.Count < 2)
            {
                _plr.SendErrorMessage("You need to specify a the faction name.");
                return false;
            }

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (_factionName.Length < SFactions.Config.MinNameLength)
            {
                _plr.SendErrorMessage($"Faction name needs to be at least {SFactions.Config.MinNameLength} characters long.");
                return false;
            }

            if (_factionName.Length > SFactions.Config.MaxNameLength)
            {
                _plr.SendErrorMessage($"Faction name needs to be at most {SFactions.Config.MaxNameLength} characters long.");
                return false;
            }

            if (SFactions.DbManager.DoesFactionExist(_factionName))
            {
                _plr.SendErrorMessage("A faction with this name already exists.");
                return false;
            }

            return true;
        }
    }
}