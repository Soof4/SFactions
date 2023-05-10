using IL.Terraria.Chat.Commands;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFactions {
    public class Config {
        public string[] factionPrefixes = {"", "[Red] ", "[Green] ", "[Blue] ", "[Yellow] ", "[Pink] "};
        public IDictionary<string, bool> subcommandsPerms = new Dictionary<string, bool>();
        public void Write() {
            subcommandsPerms.Add("help", true);
            subcommandsPerms.Add("change", true);
            File.WriteAllText(SFactionsMain.configPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        public static Config Read() {
            if (!File.Exists(SFactionsMain.configPath)) {
                return new Config();
            }
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(SFactionsMain.configPath));
        }
    }
}
