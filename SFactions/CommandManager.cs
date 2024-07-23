using TShockAPI;
using SFactions.Database;
using Abilities;
using TShockAPI.Configuration;
using TerrariaApi.Server;
using SFactions.Commands;

namespace SFactions
{
    public static class CommandManager
    {
        public static void FactionCmd(CommandArgs args)
        {
            TSPlayer player = args.Player;
            if (args.Parameters.Count < 1)
            {
                player.SendErrorMessage("You need to specify a subcommand. Do '/faction help' to see all subcommands.");
                return;
            }

            GetSubCommandInstance(args.Parameters[0].ToLower()).Execute(args);
        }

        public static AbstractCommand GetSubCommandInstance(string str)
        {
            return str switch
            {
                "create" => new CreateCommand(),
                "join" => new JoinCommand(),
                "leave" => new LeaveCommand(),
                "rename" => new RenameCommand(),
                "lead" => new LeadCommand(),
                "ability" => new AbilityCommand(),
                "region" => new RegionCommand(),
                "invitetype" => new InvitetypeCommand(),
                "invite" => new InviteCommand(),
                "accept" => new AcceptCommand(),
                "info" => new InfoCommand(),
                "base" => new BaseCommand(),
                "war" => new WarCommand(),
                _ => new HelpCommand(),
            };
        }
    }
}
