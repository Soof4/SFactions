using SFactions.Database;
using TShockAPI;

namespace SFactions
{
    public static class OnlineFactions
    {
        private static Dictionary<int, Faction> _onlineFactions = new Dictionary<int, Faction>();
        private static Dictionary<int, int> _onlineMembers = new Dictionary<int, int>();

        public static Faction GetFaction(int factionId)
        {
            return _onlineFactions[factionId];
        }

        public static bool IsFactionOnline(int factionId)
        {
            return _onlineFactions.ContainsKey(factionId);
        }

        public static int GetFactionID(int playerIndex)
        {
            return _onlineMembers[playerIndex];
        }

        public static bool IsPlayerInAnyFaction(int playerIndex)
        {
            return _onlineMembers.ContainsKey(playerIndex);
        }

        public static Faction GetPlayerFaction(int playerIndex)
        {
            return GetFaction(GetFactionID(playerIndex));
        }

        public static List<TSPlayer> GetAllMembers(int factionId)
        {
            List<TSPlayer> list = new List<TSPlayer>();

            foreach (var kvp in _onlineMembers)
            {
                if (kvp.Value == factionId)
                {
                    list.Add(TShock.Players[kvp.Key]);
                }
            }

            return list;
        }
    }
}