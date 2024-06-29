using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class InfoCommand : AbstractCommand
    {
        public override string HelpText => "Shows the information about the faction.";
        public override string SyntaxHelp => "/faction info <faction name>";
        protected override bool AllowServer => true;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _faction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plr.SendInfoMessage($"Faction ID: {_faction.Id}\n" +
                                 $"Faction Name: {_faction.Name}\n" +
                                 $"Faction Leader: {_faction.Leader}\n" +
                                 $"Faction Ability: {Utils.ToTitleCase(_faction.Ability.GetType().Name)}\n" +
                                 $"Has a Region: {_faction.Region != null}\n" +
                                 $"Invite Type: {Utils.ToTitleCase(_faction.InviteType.ToString())}"
                                 );
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            if (args.Parameters.Count < 2)
            {
                _plr.SendErrorMessage("Please specify a faction name.");
                return false;
            }

            string factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));

            if (!SFactions.DbManager.DoesFactionExist(factionName))
            {
                _plr.SendErrorMessage($"There is no faction called {factionName}.");
                return false;
            }

            _faction = SFactions.DbManager.GetFaction(factionName);

            return true;
        }
    }
}