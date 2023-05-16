using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SFactions {
    [ApiVersion(2, 1)]
    public class SFactionsMain : TerrariaPlugin {
        public override string Name => "SFactions";
        public override Version Version => new Version(0, 9, 0);
        public override string Author => "Soofa";
        public override string Description => "An experimental factions plugin.";
        public SFactionsMain(Main game) : base(game) {
        }
        public static string dbPath = Path.Combine(TShock.SavePath + "/SFactions/SFactionsDB.json");
        public static string configPath = Path.Combine(TShock.SavePath + "/SFactions/SFactionsConfig.json");
        public static DatabaseManager db = new DatabaseManager();
        public static Config Config = new Config();
        public override void Initialize() {
            PlayerHooks.PlayerChat += ChatManager.OnPlayerChat;
            GeneralHooks.ReloadEvent += OnReload;
            //ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
            //ServerApi.Hooks.NetGetData.Register(this, PvPManager.OnNetGetData);

            TShockAPI.Commands.ChatCommands.Add(new TShockAPI.Command("sfactions.faction", Commands.FactionCmd, "faction", "f") {
                AllowServer = false,
                HelpText = "To see the detailed help message do '/faction help'"
            });

            if (File.Exists(dbPath)) {
                db = DatabaseManager.Read();
            }
            else {
                db.Write();
            }

            if (File.Exists(configPath)) {
                Config = Config.Read();
            }
            else {
                Config.Write();
            }
        }

        /*
        private void OnNetGreetPlayer(GreetPlayerEventArgs args) {
            TSPlayer player = TShock.Players[args.Who];
            if (!db.players.ContainsKey(player.Name)) {
                db.players.Add(player.Name, 0);
                db.Write();
                return;
            }
            player.SetTeam(db.players[player.Name]);
        }
        */

        private void OnReload(ReloadEventArgs e) {
            if (File.Exists(dbPath)) {
                db = DatabaseManager.Read();
            }
            else {
                db.Write();
            }

            if (File.Exists(configPath)) {
                Config = Config.Read();
            }
            else {
                Config.Write();
            }
            e.Player.SendSuccessMessage("SFactions has been reloaded.");
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                PlayerHooks.PlayerChat -= ChatManager.OnPlayerChat;
                GeneralHooks.ReloadEvent -= OnReload;
                //ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnNetGreetPlayer);
                ServerApi.Hooks.NetGetData.Deregister(this, PvPManager.OnNetGetData);
            }
            base.Dispose(disposing);
        }
    }
}