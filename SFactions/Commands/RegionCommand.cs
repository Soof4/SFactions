using System.Reflection;
using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class RegionCommand : AbstractCommand
    {
        public override string HelpText => "Sets or deletes faction's region. (You must be inside an already defined region before setting it.)";
        public override string SyntaxHelp => "/faction <set / del>";
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
                _plr.SendErrorMessage("Only leader can set or delete the faction region.");
                return false;
            }

            if (args.Parameters.Count < 2)
            {
                _plr.SendErrorMessage("You did not specify \"set\" or \"del\"");
                return false;
            }

            switch (args.Parameters[1].ToLower())
            {
                case "set":
                    _subCommand = Set;
                    break;
                case "del":
                    _subCommand = Delete;
                    break;
                default:
                    _plr.SendErrorMessage("Invalid region subcommand. (Please use either \"set\" or \"del\")");
                    return false;
            }

            return true;
        }

        private void Set(CommandArgs args)
        {
            if (_plrFaction.Region != null)
            {
                _plr.SendErrorMessage("You need to delete the old region before setting new one. (Do \"/faction region del\" to delete old region.)");
                return;
            }

            if (_plr.CurrentRegion == null)
            {
                _plr.SendErrorMessage("You're not in a protected region.");
                return;
            }

            if (_plr.CurrentRegion.Owner != _plr.Name)
            {
                _plr.SendErrorMessage("You're not the owner of this region.");
                return;
            }

            RegionManager.SetRegion(_plr);
            RegionManager.AddAllMembers(SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)_plr.Index]]);
            _plr.SendSuccessMessage("Successfully set region.");
        }

        private void Delete(CommandArgs args)
        {
            RegionManager.DelAllMembers(_plrFaction);
            RegionManager.DelRegion(_plr);
            _plr.SendSuccessMessage("Successfully deleted the region.");
        }
    }
}