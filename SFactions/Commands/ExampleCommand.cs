#if false
using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class ExampleCommand : AbstractCommand
    {
        public override string HelpText => "HelpText";
        public override string SyntaxHelp => "SyntaxHelp";
        protected override bool AllowServer => false;
        
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
#endif