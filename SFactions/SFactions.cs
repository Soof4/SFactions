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
    public class SFactions : TerrariaPlugin {
        public override string Name => "SFactions";
        public override Version Version => new Version(1, 1, 2);
        public override string Author => "Soofa";
        public override string Description => "An experimental factions plugin.";
        public SFactions(Main game) : base(game) {
        }
        public static string configPath = Path.Combine(TShock.SavePath + "/SFactionsConfig.json");
        public static DatabaseManager dbManager = new(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "SFactions.sqlite")));
        public static Config Config = new();
        
        public static Dictionary<byte, int> onlineMembers = new();    // player index: faction id
        public static Dictionary<int, Faction> onlineFactions = new();    // faction id: faction
        public override void Initialize() {
            GeneralHooks.ReloadEvent += OnReload;
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            PlayerHooks.PlayerChat += ChatManager.OnPlayerChat;

            ServerApi.Hooks.NetGetData.Register(this, OnNetGetData);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnNetGreetPlayer);
            ServerApi.Hooks.NetSendData.Register(this, AbilityExtentions.RespawnCooldownBuffAdder);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            

            TShockAPI.Commands.ChatCommands.Add(new("sfactions.faction", Commands.FactionCmd, "faction", "f") {
                AllowServer = false,
                HelpText = "To see the detailed help message do \"/faction help\""
            });
            
            Config = Config.Read();
        }

        private void OnNetGetData(GetDataEventArgs args) {
            if (args.MsgID == PacketTypes.PlayerDeathV2 && onlineMembers.ContainsKey((byte)args.Msg.whoAmI)) {
                Faction plrFaction = onlineFactions[onlineMembers[(byte)args.Msg.whoAmI]];
                if (plrFaction.Ability == AbilityType.Marthymr) {
                    Abilities.Abilities.Marthymr(TShock.Players[args.Msg.whoAmI], 90, PointManager.GetAbilityLevelByBossProgression());
                }
            }
        }

        private void OnNetGreetPlayer(GreetPlayerEventArgs args) {
            try {
                Faction plrFaction = dbManager.GetPlayerFaction(TShock.Players[args.Who].Name);
                onlineMembers.Add((byte)args.Who, plrFaction.Id);
                int factionId = onlineMembers[(byte)args.Who];

                // Add the faction to onlineFactions
                foreach (int id in onlineFactions.Keys) {
                    if (id == factionId) {
                        return;
                    }
                }
                onlineFactions.Add(factionId, plrFaction);
            }
            catch (NullReferenceException) {
                return;
            }
        }

        private void OnServerLeave(LeaveEventArgs args) {
            if (onlineMembers.ContainsKey((byte)args.Who)) {
                int factionId = onlineMembers[(byte)args.Who];
                onlineMembers.Remove((byte)args.Who);

                // Remove the faction from onlineFactions if nobody else is online
                foreach (int id in onlineMembers.Values) { 
                    if (id == factionId) {
                        return;
                    }
                }
                onlineFactions.Remove(factionId);
            }
        }
        
        private void OnReload(ReloadEventArgs e) {
            Config = Config.Read();
            e.Player.SendSuccessMessage("SFactions has been reloaded.");
        }

        private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args) {
            if (args.Control.IsUsingItem && args.Player.SelectedItem.netID == ItemID.WhoopieCushion && onlineMembers.ContainsKey(args.PlayerId)) {
                Faction plrFaction = onlineFactions[onlineMembers[args.PlayerId]];
                int level = PointManager.GetAbilityLevelByBossProgression();
                switch (plrFaction.Ability) {
                    case AbilityType.DryadsRingOfHealing:
                        Abilities.Abilities.DryadsRingOfHealing(args.Player, 90, level);
                        break;
                    case AbilityType.RingOfDracula:
                        Abilities.Abilities.RingOfDracula(args.Player, 90, level);
                        break;
                    case AbilityType.SandFrames:
                        Abilities.Abilities.SandFrames(args.Player, 90, level);
                        break;
                    case AbilityType.Adrenaline:
                        Abilities.Abilities.Adrenaline(args.Player, 90, level);
                        break;
                    case AbilityType.Witch:
                        Abilities.Abilities.Witch(args.Player, 90, level);
                        break;
                    case AbilityType.RandomTeleport:
                        Abilities.Abilities.RandomTeleport(args.Player, 90, level);
                        break;
                    case AbilityType.EmpressOfLight:
                        Abilities.Abilities.EmpressOfLight(args.Player, 90, level);
                        break;
                    case AbilityType.Twilight:
                        Abilities.Abilities.Twilight(args.Player, 90, level);
                        break;
                    case AbilityType.Harvest:
                        Abilities.Abilities.Harvest(args.Player, 90, level);
                        break;
                    default: return;
                        
                }
            }
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                PlayerHooks.PlayerChat -= ChatManager.OnPlayerChat;
                GeneralHooks.ReloadEvent -= OnReload;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnNetGreetPlayer);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
                GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
            }
            base.Dispose(disposing);
        }
    }
}