using Abilities;
using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class AbilityCommand : AbstractCommand
    {
        public override string HelpText => "Changes faction's ability.";
        public override string SyntaxHelp => "/faction ability <ability name>";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;
        private AbilityType _newType;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            _plrFaction.AbilityType = _newType;
            _plrFaction.Ability = Utils.CreateAbility(_newType, Utils.GetAbilityLevel(_plrFaction));

            SFactions.DbManager.SaveFaction(SFactions.OnlineFactions[_plrFaction.Id]);

            _plr.SendSuccessMessage($"Your faction's ability is now \"{args.Parameters[1]}\".");
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
                _plr.SendErrorMessage("Only leaders can change the faction ability.");
                return false;
            }

            if (args.Parameters.Count < 2)
            {
                _plr.SendErrorMessage("Invalid ability type. Valid types are:\n" +
                                     string.Join(", ", SFactions.Config.EnabledAbilities));
                return false;
            }

            if (!Utils.TryGetAbilityTypeFromString(args.Parameters[1].ToLower(), out _newType))
            {
                _plr.SendErrorMessage("Invalid ability type. Valid types are:\n" +
                                     string.Join(", ", SFactions.Config.EnabledAbilities));
                return false;
            }

            if (!SFactions.Config.EnabledAbilities.Contains(args.Parameters[1].ToLower()))
            {
                _plr.SendErrorMessage("Invalid ability type. Valid types are:\n" +
                                     string.Join(", ", SFactions.Config.EnabledAbilities));
                return false;
            }

            return true;
        }
    }
}