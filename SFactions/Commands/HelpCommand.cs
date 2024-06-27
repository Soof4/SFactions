using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class HelpCommand : AbstractCommand
    {
        public static new string HelpText => "Shows help texts for faction commands.";
        public static new string SyntaxHelp => "/faction help [page number / command name]";
#pragma warning disable CS8618

        private TSPlayer _plr;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            // Logic here
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            return true;
        }
    }
}