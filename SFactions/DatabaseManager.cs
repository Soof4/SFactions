using Newtonsoft.Json;

namespace SFactions {
    public class DatabaseManager {
        public IDictionary<int,string> factions = new Dictionary<int,string>();
        public IDictionary<int,string> leaders = new Dictionary<int,string>();
        public IDictionary<string, int> players = new Dictionary<string, int>();
        public void Write() {
            File.WriteAllText(SFactionsMain.dbPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        public static DatabaseManager Read() {
            if (!File.Exists(SFactionsMain.dbPath)) {
                return new DatabaseManager();
            }
            return JsonConvert.DeserializeObject<DatabaseManager>(File.ReadAllText(SFactionsMain.dbPath));
        }
    }
}
