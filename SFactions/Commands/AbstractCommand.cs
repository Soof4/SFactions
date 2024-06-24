using TShockAPI;

namespace SFactions
{
    public abstract class AbstractCommand
    {
        protected delegate void SubCommand(CommandArgs args);
        protected abstract bool TryParseParameters(CommandArgs args);
        protected abstract void Function(CommandArgs args);
        public void Execute(CommandArgs args)
        {
            if (TryParseParameters(args))
            {
                Function(args);
            }
        }
    }
}