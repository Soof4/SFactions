using IL.Terraria;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFactions {
    public class DatabaseManager {
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
