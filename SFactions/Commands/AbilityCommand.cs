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

            SFactions.DbManager.SaveFaction(_plrFaction);

            _plr.SendSuccessMessage($"Your faction's ability is now \"{args.Parameters[1]}\".");
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);
            CommandParser.IsPlayerTheLeader(_plrFaction, _plr);

            string validTypes = string.Join(", ", SFactions.Config.EnabledAbilities);
            CommandParser.IsMissingArgument(args, 1, $"Missing ability name. Valid ability types are: {validTypes}");

            if (!Utils.TryGetAbilityTypeFromString(args.Parameters[1].ToLower(), out _newType))
            {
                throw new GenericCommandException($"Invalid ability name. Valid ability types are: {validTypes}");
            }

            if (!SFactions.Config.EnabledAbilities.Contains(args.Parameters[1].ToLower()))
            {
                throw new GenericCommandException($"Invalid ability name. Valid ability types are: {validTypes}");
            }
        }
    }
}