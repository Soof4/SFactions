using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class ExampleCommand : AbstractCommand
    {
        public static new string HelpText => "HelpText";
        public static new string SyntaxHelp => "SyntaxHelp";
#pragma warning disable CS8618

        private TSPlayer _plr;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            // Logic here
        }

        protected override bool TryParseParameters(CommandArgs args)
        {
            // Parsing and error handling here

            return true;
        }
    }
}