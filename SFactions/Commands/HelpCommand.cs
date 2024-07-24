using System.Reflection;
using SFactions.Database;
using TShockAPI;

namespace SFactions.Commands
{
    public class HelpCommand : AbstractCommand
    {
        public override string HelpText => "Shows help texts for faction commands.";
        public override string SyntaxHelp => "/faction help [page number / command name]";
        protected override bool AllowServer => true;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private int _pageNumber = -1;
        private AbstractCommand _command;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            if (_pageNumber == -1)
            {
                _plr.SendInfoMessage(_command.HelpText + "\n" + _command.SyntaxHelp);
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            // Get concrete command classes
            var namespaceTypes = types.Where(t =>
                t.Namespace == "SFactions.Commands" &&
                !t.IsAbstract
            ).ToList();

            int maxPage = (int)Math.Ceiling((namespaceTypes.Count - 2) / 4f) - 1;

            if (_pageNumber > maxPage)
            {
                _pageNumber = maxPage;
            }

            string msg = $"Sub-commands (Page: {_pageNumber + 1}/{maxPage + 1}):";

            int startIndex = _pageNumber * 4;
            int endIndex = Math.Min(startIndex + 4, namespaceTypes.Count - 1);

            for (int i = startIndex; i < endIndex; i++)
            {
                var type = namespaceTypes[i];

                // Skip delegate types and those not meant for instantiation
                if (typeof(MulticastDelegate).IsAssignableFrom(type))
                {
                    continue;
                }

                var instance = Activator.CreateInstance(type);
                var helpTextProperty = type.GetProperty("HelpText")!;
                string helpText = (string)helpTextProperty.GetValue(instance)!;

                msg += $"\n[c/ffffbb:{Utils.GetFirstWord(type.Name)}: {helpText}]";
            }

            _plr.SendInfoMessage(msg);
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            if (args.Parameters.Count > 1)
            {
                if (Utils.TryParseInt(args.Parameters[1], ref _pageNumber))
                {
                    if (_pageNumber < 1)
                    {
                        _pageNumber = 0;
                    }
                    else
                    {
                        _pageNumber--;
                    }
                }
                else
                {
                    _command = CommandManager.GetSubCommandInstance(args.Parameters[1]);
                }
            }
            else
            {
                _command = new HelpCommand();
            }
        }
    }
}