using TShockAPI;

namespace SFactions {
    public class Utils {
        public static void UpdateOnlineMembers() {
            foreach (var kvp in SFactions.onlineMembers) {
                SFactions.onlineMembers[kvp.Key] = SFactions.dbManager.GetPlayerFaction(kvp.Key);
            }
        }
    }
}
