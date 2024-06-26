using Microsoft.Data.Sqlite;
using SFactions.Database;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Abilities;
using Terraria.ID;

namespace SFactions
{
    [ApiVersion(2, 1)]
    public class SFactions : TerrariaPlugin
    {
        #region Plugin Info

        public override string Name => "SFactions";
        public override Version Version => new Version(1, 3, 0);
        public override string Author => "Soofa";
        public override string Description => "Soofa's Factions";

        #endregion

        public SFactions(Main game) : base(game) { }
        public static DatabaseManager DbManager = new(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "SFactions.sqlite")));
        public static Configuration Config = Configuration.Reload();
        public static Random RandomGen = new();
        public static TerrariaPlugin? Instance;
        public static Dictionary<string, Faction> Invitations { get; set; } = new Dictionary<string, Faction>();

        /// <summary>
        /// Key: Player index <br></br>
        /// Value: Faction Id
        /// </summary>
        public static Dictionary<byte, int> OnlineMembers { get; set; } = new Dictionary<byte, int>();
        
        /// <summary>
        /// Key: Faction Id <br></br>
        /// Value: Faction object
        /// </summary>
        public static Dictionary<int, Faction> OnlineFactions { get; set; } = new Dictionary<int, Faction>();

        public override void Initialize()
        {
            Handlers.InitializeHandlers(this);

            TShockAPI.Commands.ChatCommands.Add(new("sfactions.faction", Commands.FactionCmd, "faction", "f")
            {
                AllowServer = false,
                HelpText = "To see the detailed help message do \"/faction help\""
            });

            Instance = this;
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
