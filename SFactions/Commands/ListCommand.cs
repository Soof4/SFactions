using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class ListCommand : AbstractCommand
    {
        public override string HelpText => "HelpText";
        public override string SyntaxHelp => "SyntaxHelp";
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private int _pageNumber = 0;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            Task.Run(async () =>
            {
                List<Faction> factions = await SFactions.DbManager.GetAllFactionsAsync();
                int maxPage = (int)Math.Ceiling(factions.Count / 4f) - 1;

                if (_pageNumber > maxPage)
                {
                    _pageNumber = maxPage;
                }

                string msg = string.Format(Localization.ListCommand_Header, _pageNumber + 1, maxPage + 1);

                int startIndex = _pageNumber * 4;
                int endIndex = Math.Min(startIndex + 4, factions.Count);

                for (int i = startIndex; i < endIndex; i++)
                {
                    Faction f = factions[i];
                    msg += string.Format(Localization.ListCommand_EntryFormat, f.Id, f.Name, f.Leader);
                }

                msg += Localization.ListCommand_PageEnd;
                _plr.SendInfoMessage(msg);
            });
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
            }
        }
    }
}