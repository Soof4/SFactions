using TShockAPI;

namespace SFactions
{
    public abstract class AbstractCommand
    {
        protected abstract bool TryParseCommands(CommandArgs args);
        protected abstract void Function(CommandArgs args);
        public void Execute(CommandArgs args)
        {
            if (TryParseCommands(args))
            {
                Function(args);
            }
        }
    }
}