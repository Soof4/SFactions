using Abilities;
using SFactions.Database;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace SFactions
{
    public static class Handlers
    {
        public static void InitializeHandlers(TerrariaPlugin registrator)
        {
            GeneralHooks.ReloadEvent += OnReload;
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            PlayerHooks.PlayerChat += ChatManager.OnPlayerChat;

            ServerApi.Hooks.NetGreetPlayer.Register(registrator, OnNetGreetPlayer);
            ServerApi.Hooks.NetSendData.Register(registrator, Abilities.Extensions.RespawnCooldownBuffAdder);
            ServerApi.Hooks.ServerLeave.Register(registrator, OnServerLeave);
            ServerApi.Hooks.NpcKilled.Register(registrator, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Register(registrator, OnDropLoot);
            ServerApi.Hooks.NpcStrike.Register(registrator, OnNpcStrike);
        }

        public static void DisposeHandlers(TerrariaPlugin deregistrator)
        {
            GeneralHooks.ReloadEvent -= OnReload;
            GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
            PlayerHooks.PlayerChat -= ChatManager.OnPlayerChat;
            
            ServerApi.Hooks.NetGreetPlayer.Deregister(deregistrator, OnNetGreetPlayer);
            ServerApi.Hooks.NetSendData.Deregister(deregistrator, Abilities.Extensions.RespawnCooldownBuffAdder);
            ServerApi.Hooks.ServerLeave.Deregister(deregistrator, OnServerLeave);
            ServerApi.Hooks.NpcKilled.Deregister(deregistrator, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Deregister(deregistrator, OnDropLoot);
            ServerApi.Hooks.NpcStrike.Deregister(deregistrator, OnNpcStrike);
        }

        private static void OnNetGreetPlayer(GreetPlayerEventArgs args)
        {
            try
            {
                Faction plrFaction = SFactions.DbManager.GetPlayerFaction(TShock.Players[args.Who].Name);
                SFactions.OnlineMembers.Add((byte)args.Who, plrFaction.Id);

                // Add the faction to OnlineFactions
                foreach (int id in SFactions.OnlineFactions.Keys)
                {
                    if (id == plrFaction.Id)
                    {
                        return;
                    }
                }
                SFactions.OnlineFactions.Add(plrFaction.Id, plrFaction);
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        private static void OnServerLeave(LeaveEventArgs args)
        {
            if (SFactions.OnlineMembers.ContainsKey((byte)args.Who))
            {
                int factionId = SFactions.OnlineMembers[(byte)args.Who];
                SFactions.OnlineMembers.Remove((byte)args.Who);

                // Remove the faction from onlineFactions if nobody else is online
                foreach (int id in SFactions.OnlineMembers.Values)
                {
                    if (id == factionId)
                    {
                        return;
                    }
                }
                SFactions.OnlineFactions.Remove(factionId);
            }
        }

        private static void OnReload(ReloadEventArgs e)
        {
            SFactions.Config = Configuration.Reload();
            e.Player.SendSuccessMessage("SFactions has been reloaded.");
        }

        private static void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            if (args.Control.IsUsingItem && SFactions.OnlineMembers.ContainsKey(args.PlayerId))
            {
                if (args.Player.SelectedItem.netID == ItemID.Harp)
                {
                    Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[args.PlayerId]];
                    int level = PointManager.GetAbilityLevel(plrFaction);
                    int cooldown = PointManager.GetAbilityCooldownByAbilityLevel(level, 100);
                    if (plrFaction.AbilityType == AbilityType.MagicDice)
                    {
                        cooldown = 70 - SFactions.RandomGen.Next(16);
                    }
                    plrFaction.Ability.Cast(args.Player, cooldown, level);
                }
                else if (args.Player.SelectedItem.netID == ItemID.CopperWatch)
                {
                    Faction plrFaction = SFactions.OnlineFactions[SFactions.OnlineMembers[args.PlayerId]];
                    plrFaction.Ability.Cycle(args.Player);
                }
            }
        }

        private static void OnNpcKill(NpcKilledEventArgs eventArgs)
        {
            if (eventArgs.npc.rarity == 10) Abilities.Extensions.ExplosiveEffectEffect(eventArgs.npc.position, eventArgs.npc.lifeMax / 5);
        }
        private static void OnDropLoot(NpcLootDropEventArgs eventArgs)
        {
            if (Main.npc[eventArgs.NpcArrayIndex].rarity == 11) eventArgs.Handled = true;
        }

        public static void OnNpcStrike(NpcStrikeEventArgs args)
        {
            if (SFactions.OnlineMembers.ContainsKey((byte)args.Player.whoAmI) &&
                SFactions.OnlineFactions[SFactions.OnlineMembers[(byte)args.Player.whoAmI]].AbilityType == AbilityType.HyperCrit)
            {
                Hooks.OnNpcStrike_HyperCrit(args);
            }
        }
    }
}