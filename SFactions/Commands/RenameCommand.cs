using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class RenameCommand : AbstractCommand
    {
        public static new string HelpText => "Renames the faction.";
        public static new string SyntaxHelp => "/faction rename <new name>";
        
#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private string _factionName;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plrFaction.Name = _factionName;
            SFactions.OnlineFactions[_plrFaction.Id].Name = _plrFaction.Name;
            SFactions.DbManager.SaveFaction(_plrFaction);
            _plr.SendSuccessMessage($"Successfully changed faction name to \"{_factionName}\"");
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (!SFactions.OnlineMembers.ContainsKey((byte)_plr.Index))
            {
                _plr.SendErrorMessage("You're not in a faction.");
                return false;
            }

            _plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)_plr.Index]];

            if (_plr.Name != _plrFaction.Leader)
            {
                _plr.SendErrorMessage("Only leaders can change the faction name.");
                return false;
            }

            if (args.Parameters.Count < 2)
            {
                _plr.SendErrorMessage("You need to specify a faction name.");
                return false;
            }

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (SFactions.DbManager.DoesFactionExist(_factionName))
            {
                _plr.SendErrorMessage("A faction with this name already exists.");
                return false;
            }

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

            return true;
        }
    }
}