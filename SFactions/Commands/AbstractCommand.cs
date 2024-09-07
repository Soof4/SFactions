using SFactions.Exceptions;
using SFactions.i18net;
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
                args.Player.SendErrorMessage(Localization.ErrorMessage_OnlyInGameCommand);
                return;
            }

            try
            {
                ParseParameters(args);
                Function(args);
            }
            catch (GenericCommandException e)
            {
                args.Player.SendErrorMessage(e.ErrorMessage);
            }
        }
    }
}