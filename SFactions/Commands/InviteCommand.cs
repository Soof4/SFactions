using SFactions.Database;
using SFactions.Exceptions;
using TShockAPI;

namespace SFactions.Commands
{
    public class InviteCommand : AbstractCommand
    {
        public override string HelpText => "Sends an invitation to another player.";
        public override string SyntaxHelp => "/faction invite <player name>";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private TSPlayer _targetPlr;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _targetPlr.SendInfoMessage($"{_plr.Name} has invited you to {_plrFaction.Name}. Type \"/faction accept\" to join. Do nothing if you don't want to join.");
            _plr.SendSuccessMessage($"You've successfully invited {_targetPlr.Name} to your faction.");
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);


            if (_plrFaction.InviteType == InviteType.Closed && !_plr.Name.Equals(_plrFaction.Leader))
            {
                throw new GenericCommandException("Only leader can invite new people.");
            }

            CommandParser.IsMissingArgument(args, 1, "Please specify a player name.");

            string targetPlrName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));

            _targetPlr = CommandParser.FindPlayer(targetPlrName);


            if (SFactions.Invitations.ContainsKey(_targetPlr.Name))
            {
                if (SFactions.Invitations[_targetPlr.Name].Id == _plrFaction.Id)
                {
                    throw new GenericCommandException("This player already has a pending invitation from your faction.");
                }
                else
                {
                    SFactions.Invitations[_targetPlr.Name] = _plrFaction;
                }
            }
            else
            {
                SFactions.Invitations.Add(_targetPlr.Name, _plrFaction);
            }

        }
    }
}