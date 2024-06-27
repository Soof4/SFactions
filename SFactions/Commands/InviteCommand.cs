using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class InviteCommand : AbstractCommand
    {
        public static new string HelpText => "Sends an invitation to another player.";
        public static new string SyntaxHelp => "/faction invite <player name>";

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

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            if (!SFactions.OnlineMembers.ContainsKey((byte)_plr.Index))
            {
                _plr.SendErrorMessage("You're not in a faction.");
                return false;
            }

            _plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)_plr.Index]];

            if (_plrFaction.InviteType == InviteType.Closed && !_plr.Name.Equals(_plrFaction.Leader))
            {
                _plr.SendErrorMessage("Only leader can invite new people.");
                return false;
            }

            string targetPlrName = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));

            TSPlayer? target = null;
            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active)
                {
                    if (p.Name.Equals(targetPlrName))
                    {
                        target = p;
                        break;
                    }
                    else if (p.Name.StartsWith(targetPlrName))
                    {
                        target = p;
                    }
                }
            }

            if (target == null)
            {
                _plr.SendErrorMessage("Couldn't find the player.");
                return false;
            }

            _targetPlr = target;

            if (SFactions.Invitations.ContainsKey(_targetPlr.Name))
            {
                if (SFactions.Invitations[_targetPlr.Name].Id == _plrFaction.Id)
                {
                    _plr.SendErrorMessage("This player already has a pending invitation from your faction.");
                    return false;
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

            return true;
        }
    }
}