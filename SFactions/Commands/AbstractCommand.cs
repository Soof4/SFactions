using TShockAPI;

namespace SFactions.Commands
{
    public abstract class AbstractCommand
    {
        public abstract string HelpText { get; }
        public abstract string SyntaxHelp { get; }
        protected abstract bool AllowServer { get; }
        protected delegate void SubCommand(CommandArgs args);
        protected abstract void ParseParameters(CommandArgs args);
        protected abstract void Function(CommandArgs args);

        public void Execute(CommandArgs args)
        {
            if (!args.Player.RealPlayer && !AllowServer)
            {
                args.Player.SendErrorMessage("You can only use this command in game.");
                return;
            }

            try
            {
                ParseParameters(args);
            }
            catch (GenericCommandException e)
            {
                args.Player.SendErrorMessage(e.ErrorMessage);
            }
            catch
            {
                args.Player.SendErrorMessage("Unknown error, please report this to developer.");
            }
        }
    }
}