using SFactions.Database;
using SFactions.Exceptions;
using SFactions.i18net;
using TShockAPI;

namespace SFactions.Commands
{
    public class DeleteCommand : AbstractCommand
    {
        public override string HelpText => Localization.DeleteCommand_HelpText;
        public override string SyntaxHelp => Localization.DeleteCommand_SyntaxtHelp;
        protected override bool AllowServer => false;

#pragma warning disable CS8618

        private TSPlayer _plr;
        private Faction _plrFaction;

#pragma warning restore CS8618

        protected override void Function(CommandArgs args)
        {
            List<TSPlayer> allOnlineMembers = FactionService.GetAllMembers(_plrFaction.Id);

            foreach (TSPlayer p in allOnlineMembers)
            {
                FactionService.RemoveMember(p);
            }

            Task.Run(async () =>
            {
                List<string> allMembers = await SFactions.DbManager.GetAllMembersAsync(_plrFaction.Id);

                foreach (string p in allMembers)
                {
                    await SFactions.DbManager.DeleteMemberAsync(p);
                }

                await SFactions.DbManager.DeleteFactionAsync(_plrFaction.Id);

                _plr.SendSuccessMessage(Localization.DeleteCommand_SuccessMessage);
            });
        }

        protected override void ParseParameters(CommandArgs args)
        {
            _plr = args.Player;

            _plrFaction = CommandParser.GetPlayerFaction(args);

            CommandParser.IsPlayerTheLeader(_plrFaction, _plr);
        }
    }
}