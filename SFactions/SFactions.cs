using Microsoft.Data.Sqlite;
using SFactions.Database;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Abilities;
using Terraria.ID;
using SFactions.i18net;

namespace SFactions
{
    [ApiVersion(2, 1)]
    public class SFactions : TerrariaPlugin
    {
        #region Plugin Info

        public override string Name => "SFactions";
        public override Version Version => new Version(2, 0, 0);
        public override string Author => "Soofa";
        public override string Description => Localization.PluginDescription;

        #endregion

        public SFactions(Main game) : base(game) => Instance = this;
        public static DatabaseManager DbManager = new(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "SFactions.sqlite")));
        public static Configuration Config = Configuration.Reload();
        public static Random RandomGen = new();
        public static TerrariaPlugin? Instance;
        public static Dictionary<string, Faction> Invitations { get; set; } = new Dictionary<string, Faction>();

        public override void Initialize()
        {
            Handlers.InitializeHandlers(this);
            LocalizationManager.LoadLanguage(Config.Language);


            TShockAPI.Commands.ChatCommands.Add(new("sfactions.faction", CommandManager.FactionCmd, "faction", "f")
            {
                HelpText = Localization.FactionCommandHelpText
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Handlers.DisposeHandlers(this);
            }
            base.Dispose(disposing);
        }
    }
}
