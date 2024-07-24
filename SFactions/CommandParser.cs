using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public static class CommandParser
    {
        public static void IsPlayerInAnyFaction(CommandArgs args)
        {
            if (!FactionService.IsPlayerInAnyFaction(args.Player))
            {
                throw new PlayerNotInFactionException();
            }
        }

        public static Faction GetPlayerFaction(CommandArgs args)
        {
            if (!FactionService.IsPlayerInAnyFaction(args.Player))
            {
                throw new PlayerNotInFactionException();
            }
            return FactionService.GetPlayerFaction(args.Player);
        }

        public static void IsPlayerTheLeader(Faction faction, TSPlayer player)
        {
            if (player.Name != faction.Leader)
            {
                throw new PlayerNotLeaderException();
            }
        }

        public static void IsMissingArgument(CommandArgs args, int neededArgCount, string errorMsg = "Missing an argument.")
        {
            if (args.Parameters.Count < neededArgCount + 1)
            {
                throw new MissingArgumentException(errorMsg);
            }
        }

        public static TSPlayer FindPlayer(string name)
        {
            TSPlayer? plr = null;

            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active)
                {
                    if (p.Name.Equals(name))
                    {
                        plr = p;
                        break;
                    }
                    else if (p.Name.StartsWith(name))
                    {
                        plr = p;
                    }
                }
            }

            if (plr == null)
            {
                throw new PlayerNotFoundException();
            }

            return plr;
        }
    }
}