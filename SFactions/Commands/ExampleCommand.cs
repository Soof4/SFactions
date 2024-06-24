using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public class ExampleCommand : AbstractCommand
    {

#pragma warning disable CS8618

        private TSPlayer _plr;

#pragma warning disable CS8618

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