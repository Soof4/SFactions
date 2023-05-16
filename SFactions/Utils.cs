using TShockAPI;

namespace SFactions {
    public class Utils {

        public static string GetFactionName(CommandArgs args) {
            if (args.Parameters.Count < 2) {
                return "";
            }

            if (args.Parameters.Count == 2) {
                return args.Parameters[1];
            }

            else {
                string name = "";
                for(int i=1; i<args.Parameters.Count; i++) {
                    name += $"{args.Parameters[i]} ";
                }
                return name.TrimEnd();
            }
        }

        public static int FindFactionName(string name) {
            foreach(var kvp in SFactionsMain.db.factions) {
                if(kvp.Value.Equals(name)) {
                    return kvp.Key;
                }
            }
            return -1;
        }
    }
}
