using TShockAPI;

namespace SFactions.Commands
{
    public abstract class AbstractCommand
    {
        public abstract string HelpText { get; }
        public abstract string SyntaxHelp { get; }
        protected abstract bool AllowServer { get; }
        protected delegate void SubCommand(CommandArgs args);
        protected abstract bool TryParseParameters(CommandArgs args);
        protected abstract void Function(CommandArgs args);
        public void Execute(CommandArgs args)
        {
            if ((args.Player.RealPlayer || AllowServer) && TryParseParameters(args))
            {
                Function(args);
            }
        }
    }
}