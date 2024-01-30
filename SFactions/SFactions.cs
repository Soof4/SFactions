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
        public override string Name => "SFactions";
        public override Version Version => new Version(1, 2, 3);
        public override string Author => "Soofa";
        public override string Description => "Sausage Factions? Smexy Factions? Sup Factions?";
        public SFactions(Main game) : base(game) { }
        public static string ConfigPath = Path.Combine(TShock.SavePath + "/SFactionsConfig.json");
        public static DatabaseManager DbManager = new(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "SFactions.sqlite")));
        public static Config Config = new();

        public static Random RandomGen = new();

        /// <summary>
        /// Key: Player index <br></br>
        /// Value: Faction Id
        /// </summary>
        public static Dictionary<byte, int> OnlineMembers = new();
        /// <summary>
        /// Key: Faction Id <br></br>
        /// Value: Faction object
        /// </summary>
        public static Dictionary<int, Faction> OnlineFactions = new();

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += OnReload;
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            PlayerHooks.PlayerChat += ChatManager.OnPlayerChat;

            ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
            ServerApi.Hooks.NetSendData.Register(this, Abilities.Extensions.RespawnCooldownBuffAdder);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Register(this, OnDropLoot);

            TShockAPI.Commands.ChatCommands.Add(new("sfactions.faction", Commands.FactionCmd, "faction", "f")
            {
                AllowServer = false,
                HelpText = "To see the detailed help message do \"/faction help\""
            });

            Config = Config.Read();
        }

        private void OnNetGreetPlayer(GreetPlayerEventArgs args)
        {
            try
            {
                Faction plrFaction = DbManager.GetPlayerFaction(TShock.Players[args.Who].Name);
                OnlineMembers.Add((byte)args.Who, plrFaction.Id);

                // Add the faction to OnlineFactions
                foreach (int id in OnlineFactions.Keys)
                {
                    if (id == plrFaction.Id)
                    {
                        return;
                    }
                }
                OnlineFactions.Add(plrFaction.Id, plrFaction);
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        private void OnServerLeave(LeaveEventArgs args)
        {
            if (OnlineMembers.ContainsKey((byte)args.Who))
            {
                int factionId = OnlineMembers[(byte)args.Who];
                OnlineMembers.Remove((byte)args.Who);

                // Remove the faction from onlineFactions if nobody else is online
                foreach (int id in OnlineMembers.Values)
                {
                    if (id == factionId)
                    {
                        return;
                    }
                }
                OnlineFactions.Remove(factionId);
            }
        }

        private void OnReload(ReloadEventArgs e)
        {
            Config = Config.Read();
            e.Player.SendSuccessMessage("SFactions has been reloaded.");
        }

        private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            if (args.Player.SelectedItem.netID == ItemID.Harp && args.Control.IsUsingItem && OnlineMembers.ContainsKey(args.PlayerId))
            {
                Faction plrFaction = OnlineFactions[OnlineMembers[args.PlayerId]];
                int level = PointManager.GetAbilityLevel(plrFaction);
                int cooldown = PointManager.GetAbilityCooldownByAbilityLevel(level, 100);
                if (plrFaction.AbilityType == AbilityType.MagicDice) {
                    cooldown = 70 - RandomGen.Next(16);
                }
                plrFaction.Ability.Cast(args.Player, cooldown, level);
            }
        }

        private void OnNpcKill(NpcKilledEventArgs eventArgs)
        {
            if (eventArgs.npc.rarity == 10) Abilities.Extensions.ExplosiveEffectEffect(eventArgs.npc.position, eventArgs.npc.lifeMax / 5);
        }
        private void OnDropLoot(NpcLootDropEventArgs eventArgs)
        {
            if (Main.npc[eventArgs.NpcArrayIndex].rarity == 11) eventArgs.Handled = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= OnReload;
                GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
                PlayerHooks.PlayerChat -= ChatManager.OnPlayerChat;
;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnNetGreetPlayer);
                ServerApi.Hooks.NetSendData.Deregister(this, Abilities.Extensions.RespawnCooldownBuffAdder);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKill);
            }
            base.Dispose(disposing);
        }
    }
}
