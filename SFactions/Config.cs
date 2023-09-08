using Newtonsoft.Json;

namespace SFactions {
    public class Config {
        public int MinNameLength = 2;
        public int MaxNameLength = 20;
        public string ChatFormat = "{5}{1}{2}{3}: {4}";
        private string ChatFormatHelp = "{5} = Faction name, {1} = Prefix of player's group, {2} = Player's name, {3} = Suffix of player's group, {4} = Message";
        // public Dictionary<int, int> Quests = new();
        public void Write() {
            File.WriteAllText(SFactions.configPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        public static Config Read() {
            if (File.Exists(SFactions.configPath)) {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(SFactions.configPath));
                
            }
            Config newConfig = new();
            newConfig.Write();
            return newConfig;
        }
    }
}
