using Abilities;
using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class LeaveCommand : AbstractCommand
    {
        public static new string HelpText => "Used for leaving your current faction.";
        public static new string SyntaxHelp => "/faction leave";
        
#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            if (_plrFaction.AbilityType == AbilityType.TheBound)
            {
                TheBound.Pairs.Remove(args.Player);
            }

            RegionManager.DelMember(args.Player);
            SFactions.OnlineMembers.Remove((byte)args.Player.Index);
            SFactions.DbManager.DeleteMember(args.Player.Name);

            args.Player.SendSuccessMessage("You've left your faction.");

            if (args.Player.Name.Equals(_plrFaction.Leader))
            {
                _plrFaction.Leader = null;
                SFactions.DbManager.SaveFaction(_plrFaction);
                SFactions.OnlineFactions[_plrFaction.Id].Leader = null;


                // Check if anyone else is in the same faction and online
                foreach (int id in SFactions.OnlineMembers.Values)
                {
                    if (id == _plrFaction.Id)
                    {
                        return;
                    }
                }

                // If no other member is online, remove the faction from OnlineFactions.
                SFactions.OnlineFactions.Remove(_plrFaction.Id);  
            }
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (!SFactions.OnlineMembers.ContainsKey((byte)args.Player.Index))
            {
                args.Player.SendErrorMessage("You're not in a faction.");
                return false;
            }

            _plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.Index]];

            return true;
        }
    }
}