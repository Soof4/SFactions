using Newtonsoft.Json;

namespace SFactions {
    public class Config {
        public int maxNameLength = 20;
        public IDictionary<string, bool> subcommandsPerms = new Dictionary<string, bool>();
        public void Write() {
            subcommandsPerms.TryAdd("help", true);
            subcommandsPerms.TryAdd("create", true);
            subcommandsPerms.TryAdd("join", true);
            subcommandsPerms.TryAdd("leave", true);
            subcommandsPerms.TryAdd("rename", true);
            subcommandsPerms.TryAdd("lead", true);

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
