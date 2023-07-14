using Microsoft.Data.Sqlite;
using SFactions.Database;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SFactions
{
    [ApiVersion(2, 1)]
    public class SFactions : TerrariaPlugin {
        public override string Name => "SFactions";
        public override Version Version => new Version(0, 10, 6);
        public override string Author => "Soofa";
        public override string Description => "An experimental factions plugin.";
        public SFactions(Main game) : base(game) {
        }
        public static string configPath = Path.Combine(TShock.SavePath + "/SFactionsConfig.json");
        public static DatabaseManager dbManager = new(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "SFactions.sqlite")));
        public static Config Config = new();

        public static Dictionary<string, Faction> onlineMembers = new();
        public override void Initialize() {
            PlayerHooks.PlayerChat += ChatManager.OnPlayerChat;
            GeneralHooks.ReloadEvent += OnReload;
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);

            TShockAPI.Commands.ChatCommands.Add(new TShockAPI.Command("sfactions.faction", Commands.FactionCmd, "faction", "f") {
                AllowServer = false,
                HelpText = "To see the detailed help message do \"/faction help\""
            });

            Config = Config.Read();
        }

        private void OnServerLeave(LeaveEventArgs args) {
            try {
                onlineMembers.Remove(TShock.Players[args.Who].Name);
            }
            catch (ArgumentNullException) { return; }
        }

        private void OnNetGreetPlayer(GreetPlayerEventArgs args) {
            try {
                onlineMembers.Add(TShock.Players[args.Who].Name, dbManager.GetPlayerFaction(TShock.Players[args.Who].Name));
            }
            catch (NullReferenceException) { return; }
        }
        

        private void OnReload(ReloadEventArgs e) {
            Config = Config.Read();
            e.Player.SendSuccessMessage("SFactions has been reloaded.");
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                PlayerHooks.PlayerChat -= ChatManager.OnPlayerChat;
                GeneralHooks.ReloadEvent -= OnReload;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnNetGreetPlayer);
                //ServerApi.Hooks.NetGetData.Deregister(this, PvPManager.OnNetGetData);
            }
            base.Dispose(disposing);
        }
    }
}