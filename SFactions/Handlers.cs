using Abilities;
using SFactions.Database;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Abilities.Abilities;
using Abilities.Enums;
using SFactions.i18net;

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
            ServerApi.Hooks.NetSendData.Register(registrator, Abilities.Utils.RespawnCooldownBuffAdder);
            ServerApi.Hooks.ServerLeave.Register(registrator, OnServerLeave);
            ServerApi.Hooks.NpcKilled.Register(registrator, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Register(registrator, OnDropLoot);
            ServerApi.Hooks.NpcStrike.Register(registrator, OnNpcStrike);
            ServerApi.Hooks.GamePostInitialize.Register(registrator, OnGamePostInitialize);
        }

        public static void DisposeHandlers(TerrariaPlugin deregistrator)
        {
            GeneralHooks.ReloadEvent -= OnReload;
            GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
            PlayerHooks.PlayerChat -= ChatManager.OnPlayerChat;

            ServerApi.Hooks.NetGreetPlayer.Deregister(deregistrator, OnNetGreetPlayer);
            ServerApi.Hooks.NetSendData.Deregister(deregistrator, Abilities.Utils.RespawnCooldownBuffAdder);
            ServerApi.Hooks.ServerLeave.Deregister(deregistrator, OnServerLeave);
            ServerApi.Hooks.NpcKilled.Deregister(deregistrator, OnNpcKill);
            ServerApi.Hooks.NpcLootDrop.Deregister(deregistrator, OnDropLoot);
            ServerApi.Hooks.NpcStrike.Deregister(deregistrator, OnNpcStrike);
            ServerApi.Hooks.GamePostInitialize.Deregister(deregistrator, OnGamePostInitialize);
        }

        private static void OnGamePostInitialize(EventArgs args)
        {
            UpdateManager.CheckUpdateVerbose(SFactions.Instance);
        }

        private static void OnNetGreetPlayer(GreetPlayerEventArgs args)
        {
            HyperCrit.HyperCritActive.Add((byte)args.Who, 0);

            Task.Run(async () =>
            {
                Faction plrFaction = await SFactions.DbManager.GetPlayerFactionAsync(TShock.Players[args.Who].Name);

                if (!FactionService.IsFactionOnline(plrFaction.Id))
                {
                    FactionService.AddFaction(plrFaction);
                }

                FactionService.TryAddMember(args.Who, plrFaction);
            });
        }

        private static void OnServerLeave(LeaveEventArgs args)
        {
            HyperCrit.HyperCritActive.Remove((byte)args.Who);

            if (FactionService.IsPlayerInAnyFaction(args.Who))
            {
                int factionId = FactionService.GetFactionID(args.Who);
                FactionService.RemoveMember(args.Who);

                // Remove the faction from onlineFactions if nobody else is online
                if (!FactionService.IsAnyoneOnline(factionId)) FactionService.RemoveFaction(factionId);
            }
        }

        private static void OnReload(ReloadEventArgs e)
        {
            SFactions.Config = Configuration.Reload();
            e.Player.SendSuccessMessage(Localization.ReloadSuccessMessage);
        }

        private static void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            if (args.Control.IsUsingItem && FactionService.IsPlayerInAnyFaction(args.PlayerId))
            {
                if (args.Player.SelectedItem.netID == ItemID.Harp)
                {
                    Faction plrFaction = FactionService.GetFaction(args.Player);
                    int level = Utils.GetAbilityLevel(plrFaction);
                    int cooldown = Utils.GetAbilityCooldownByAbilityLevel(level, SFactions.Config.BaseAbilityCooldown);

                    if (plrFaction.AbilityType == AbilityType.MagicDice)
                    {
                        cooldown = 70 - SFactions.RandomGen.Next(16);
                    }

                    plrFaction.Ability.Cast(args.Player, cooldown, level);
                }
                else if (args.Player.SelectedItem.netID == ItemID.CopperWatch)
                {
                    Faction plrFaction = FactionService.GetFaction(args.Player);
                    plrFaction.Ability.Cycle(args.Player);
                }
            }
        }

        private static void OnNpcKill(NpcKilledEventArgs eventArgs)
        {
            if (eventArgs.npc.rarity == 10) Abilities.Utils.ExplosiveEffectEffect(eventArgs.npc.position, eventArgs.npc.lifeMax / 5);
        }

        private static void OnDropLoot(NpcLootDropEventArgs eventArgs)
        {
            if (Main.npc[eventArgs.NpcArrayIndex].rarity == 11) eventArgs.Handled = true;
        }

        public static void OnNpcStrike(NpcStrikeEventArgs args)
        {
            if (FactionService.IsPlayerInAnyFaction(args.Player.whoAmI))
            {
                Faction faction = FactionService.GetPlayerFaction(args.Player.whoAmI);

                if (faction.AbilityType == AbilityType.HyperCrit)
                {
                    Hooks.OnNpcStrike_HyperCrit(args, true);
                }
                else
                {
                    Hooks.OnNpcStrike_HyperCrit(args, false);
                }

                // else Hooks.OnNpcStrike_HyperCrit(args, false);
            }
            else Hooks.OnNpcStrike_HyperCrit(args, false);
        }
    }
}
