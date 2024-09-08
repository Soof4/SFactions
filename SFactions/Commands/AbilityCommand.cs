using Abilities;
using SFactions.Database;
using SFactions.Exceptions;
using TShockAPI;
using Abilities.Enums;
using Abilities.Abilities;
using SFactions.i18net;

namespace SFactions.Commands
{
    public class AbilityCommand : AbstractCommand
    {
        public override string HelpText => Localization.AbilityCommand_HelpText;
        public override string SyntaxHelp => Localization.AbilityCommand_SyntaxHelp;
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

            _ = SFactions.DbManager.SaveFactionAsync(_plrFaction);
            _plr.SendSuccessMessage(string.Format(Localization.AbilityCommand_SuccessMessage, args.Parameters[1]));
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;
            _plrFaction = CommandParser.GetPlayerFaction(args);
            CommandParser.IsPlayerTheLeader(_plrFaction, _plr);

            string validTypes = "\n";
            int i = 0;

            foreach (string str in SFactions.Config.EnabledAbilities)
            {
                if (i == 3)
                {
                    validTypes += "\n";
                    i = 0;
                }

                validTypes += str + ", ";
                i++;
            }

            validTypes = validTypes[..^2];

            CommandParser.IsMissingArgument(args, 1, string.Format(Localization.AbilityCommand_ErrorMessage_MissingAbilityName, validTypes));

            if (!Utils.TryGetAbilityTypeFromString(args.Parameters[1].ToLower(), out _newType))
            {
                throw new GenericCommandException(string.Format(Localization.AbilityCommand_ErrorMessage_InvalidAbilityName, validTypes));
            }

            if (!SFactions.Config.EnabledAbilities.Contains(args.Parameters[1].ToLower()))
            {
                throw new GenericCommandException(string.Format(Localization.AbilityCommand_ErrorMessage_InvalidAbilityName, validTypes));
            }
        }
    }
}