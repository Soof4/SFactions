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

        public static Faction GetFaction(TSPlayer player)
        {
            return _onlineFactions[_onlineMembers[player.Index]];
        }

        public static void AddFaction(Faction faction)
        {
            _onlineFactions.Add(faction.Id, faction);
        }

        public static void RemoveFaction(Faction faction)
        {
            _onlineFactions.Remove(faction.Id);
        }

        public static void RemoveFaction(int factionId)
        {
            _onlineFactions.Remove(factionId);
        }

        public static bool IsFactionOnline(int factionId)
        {
            return _onlineFactions.ContainsKey(factionId);
        }

        public static bool IsFactionOnline(Faction faction)
        {
            return _onlineFactions.ContainsKey(faction.Id);
        }

        public static int GetFactionID(int playerIndex)
        {
            return _onlineMembers[playerIndex];
        }

        public static bool IsPlayerInAnyFaction(int playerIndex)
        {
            return _onlineMembers.ContainsKey(playerIndex);
        }

        public static bool IsPlayerInAnyFaction(TSPlayer player)
        {
            return _onlineMembers.ContainsKey(player.Index);
        }

        public static Faction GetPlayerFaction(int playerIndex)
        {
            return GetFaction(GetFactionID(playerIndex));
        }

        public static Faction GetPlayerFaction(TSPlayer player)
        {
            return GetFaction(GetFactionID(player.Index));
        }

        public static void AddMember(TSPlayer player, Faction faction)
        {
            _onlineMembers.Add(player.Index, faction.Id);
        }

        public static void AddMember(int playerIndex, Faction faction)
        {
            _onlineMembers.Add(playerIndex, faction.Id);
        }

        public static void RemoveMember(TSPlayer player)
        {
            _onlineMembers.Remove(player.Index);
        }

        public static void RemoveMember(int playerIndex)
        {
            _onlineMembers.Remove(playerIndex);
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

        public static bool IsAnyoneOnline(Faction faction)
        {
            return _onlineMembers.ContainsValue(faction.Id);
        }

        public static bool IsAnyoneOnline(int factionId)
        {
            return _onlineMembers.ContainsValue(factionId);
        }

        public static Faction? FindFaction(string name)
        {
            Faction? faction = null;

            foreach (Faction f in _onlineFactions.Values)
            {
                if (f.Name == name)
                {
                    faction = f;
                    break;
                }
                else if (f.Name.StartsWith(name))
                {
                    faction = f;
                }
            }

            return faction;
        }

        public static TSPlayer? GetLeader(Faction faction)
        {
            TSPlayer? leader = null;
            
            foreach (var kvp in _onlineMembers)
            {
                if (kvp.Value == faction.Id && TShock.Players[kvp.Key].Name == faction.Leader)
                {
                    leader = TShock.Players[kvp.Key];
                    break;
                }
            }

            return leader;
        }
    }
}