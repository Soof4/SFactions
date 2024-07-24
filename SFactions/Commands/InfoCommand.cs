using SFactions.Database;
using SFactions.Exceptions;
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

                _plr.SendInfoMessage($"Faction ID: {_faction.Id}\n" +
                                     $"Faction Name: {_faction.Name}\n" +
                                     $"Faction Leader: {_faction.Leader}\n" +
                                     $"Faction Ability: {Utils.ToTitleCase(_faction.Ability.GetType().Name)}\n" +
                                     $"Has a Region: {_faction.Region != null}\n" +
                                     $"Invite Type: {Utils.ToTitleCase(_faction.InviteType.ToString())}"
                                     );
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

            CommandParser.IsMissingArgument(args, 1, "Please specify a faction name.");

            _factionName = string.Join(' ', args.Parameters.GetRange(1, args.Parameters.Count - 1));
        }
    }
}