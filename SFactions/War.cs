using SFactions.Database;
using Steamworks;
using TerrariaApi.Server;
using TShockAPI;


namespace SFactions
{
    public class War
    {
        public Faction Faction1 { get; set; }
        public Faction Faction2 { get; set; }
        public int KillCount1 { get; set; }
        public int KillCount2 { get; set; }
        public DateTime StartTime { get; set; }

        public War(Faction faction1, Faction faction2)
        {
            Faction1 = faction1;
            Faction2 = faction2;
            KillCount1 = 0;
            KillCount2 = 0;
            StartTime = DateTime.UtcNow;
        }

        public async void Start()
        {
            if (WarCommand.ActiveWar == null) return;

            TSPlayer.All.SendInfoMessage($"{WarCommand.ActiveWar.Faction1.Name} vs {WarCommand.ActiveWar.Faction2.Name} war has started!");

            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active && SFactions.OnlineMembers.ContainsKey((byte)p.Index))
                {
                    if (SFactions.OnlineMembers[(byte)p.Index] == WarCommand.ActiveWar.Faction1.Id)
                    {
                        p.TPlayer.team = 1;
                        TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: p.Index);
                        p.TPlayer.hostile = true;
                        TSPlayer.All.SendData(PacketTypes.TogglePvp, number: p.Index);
                    }
                    else if (SFactions.OnlineMembers[(byte)p.Index] == WarCommand.ActiveWar.Faction2.Id)
                    {
                        p.TPlayer.team = 2;
                        TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: p.Index);
                        p.TPlayer.hostile = true;
                        TSPlayer.All.SendData(PacketTypes.TogglePvp, number: p.Index);
                    }
                }
            }

            ServerApi.Hooks.NetGreetPlayer.Register(SFactions.Instance, OnNetGreetPlayer_War);
            GetDataHandlers.KillMe += OnKillMe_War;
            GetDataHandlers.PlayerTeam += OnPlayerChangeTeam_War;
            GetDataHandlers.TogglePvp += OnPlayerTogglePvP_War;

            await Task.Delay(1 * 60 * 1000);
            TSPlayer.All.SendInfoMessage($"{WarCommand.ActiveWar.Faction1.Name}: {WarCommand.ActiveWar.KillCount1} vs {WarCommand.ActiveWar.Faction2.Name}: {WarCommand.ActiveWar.KillCount2}");

            await Task.Delay(1 * 60 * 1000);
            TSPlayer.All.SendInfoMessage($"{WarCommand.ActiveWar.Faction1.Name}: {WarCommand.ActiveWar.KillCount1} vs {WarCommand.ActiveWar.Faction2.Name}: {WarCommand.ActiveWar.KillCount2}");

            await Task.Delay(1 * 60 * 1000);
            TSPlayer.All.SendInfoMessage($"{WarCommand.ActiveWar.Faction1.Name}: {WarCommand.ActiveWar.KillCount1} vs {WarCommand.ActiveWar.Faction2.Name}: {WarCommand.ActiveWar.KillCount2}");


            if (WarCommand.ActiveWar.KillCount1 == WarCommand.ActiveWar.KillCount2)
            {
                TSPlayer.All.SendSuccessMessage($"The war between {WarCommand.ActiveWar.Faction1.Name} and {WarCommand.ActiveWar.Faction2.Name} ended as a TIE!");
            }
            else if (WarCommand.ActiveWar.KillCount1 > WarCommand.ActiveWar.KillCount2)
            {
                TSPlayer.All.SendSuccessMessage($"The war between {WarCommand.ActiveWar.Faction1.Name} and {WarCommand.ActiveWar.Faction2.Name} ended as {WarCommand.ActiveWar.Faction1.Name} being victorious!");

                foreach (var kvp in SFactions.OnlineMembers)
                {
                    if (kvp.Value == WarCommand.ActiveWar.Faction1.Id)
                    {
                        TSPlayer p = TShock.Players[kvp.Key];
                        string cmd = SFactions.Config.FactionWarWinCommand.Replace("%playername%", p.Name);
                        TShockAPI.Commands.HandleCommand(TSPlayer.Server, cmd);
                    }
                }
            }
            else
            {
                TSPlayer.All.SendSuccessMessage($"The war between {WarCommand.ActiveWar.Faction1.Name} and {WarCommand.ActiveWar.Faction2.Name} ended as {WarCommand.ActiveWar.Faction2.Name} being victorious!");
            }

            ServerApi.Hooks.NetGreetPlayer.Deregister(SFactions.Instance, OnNetGreetPlayer_War);
            GetDataHandlers.KillMe -= OnKillMe_War;
            GetDataHandlers.PlayerTeam -= OnPlayerChangeTeam_War;
            GetDataHandlers.TogglePvp -= OnPlayerTogglePvP_War;

            WarCommand.ActiveWar = null;
        }


        #region Handlers

        private static void OnKillMe_War(object? sender, GetDataHandlers.KillMeEventArgs args)
        {
            if (!SFactions.OnlineMembers.ContainsKey(args.PlayerId)) return;

            int fId = SFactions.OnlineMembers[args.PlayerId];

            if (fId == WarCommand.ActiveWar!.Faction1.Id)
            {
                WarCommand.ActiveWar!.KillCount2++;
            }
            else if (fId == WarCommand.ActiveWar!.Faction2.Id)
            {
                WarCommand.ActiveWar!.KillCount1++;
            }
        }

        private static void OnPlayerChangeTeam_War(object? sender, GetDataHandlers.PlayerTeamEventArgs args)
        {
            if (!SFactions.OnlineMembers.ContainsKey(args.PlayerId)) return;

            int fId = SFactions.OnlineMembers[args.PlayerId];

            if (fId == WarCommand.ActiveWar!.Faction1.Id)
            {
                args.Handled = true;
                args.Player.TPlayer.team = 1;
                TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: args.PlayerId);
            }
            else if (fId == WarCommand.ActiveWar!.Faction2.Id)
            {
                args.Handled = true;
                args.Player.TPlayer.team = 2;
                TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: args.PlayerId);
            }
        }

        private static void OnPlayerTogglePvP_War(object? sender, GetDataHandlers.TogglePvpEventArgs args)
        {
            if (!SFactions.OnlineMembers.ContainsKey(args.PlayerId)) return;

            int fId = SFactions.OnlineMembers[args.PlayerId];

            if (fId == WarCommand.ActiveWar!.Faction1.Id || fId == WarCommand.ActiveWar!.Faction2.Id)
            {
                args.Handled = true;
                args.Player.TPlayer.hostile = true;
                TSPlayer.All.SendData(PacketTypes.TogglePvp, number: args.PlayerId);
            }
        }

        private static void OnNetGreetPlayer_War(GreetPlayerEventArgs args)
        {
            if (!SFactions.OnlineMembers.ContainsKey((byte)args.Who)) return;

            int fId = SFactions.OnlineMembers[(byte)args.Who];

            TSPlayer plr = TShock.Players[args.Who];

            if (fId == WarCommand.ActiveWar!.Faction1.Id)
            {
                plr.TPlayer.team = 1;
                TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: args.Who);
                plr.TPlayer.hostile = true;
                TSPlayer.All.SendData(PacketTypes.TogglePvp, number: args.Who);
            }
            else if (fId == WarCommand.ActiveWar!.Faction2.Id)
            {
                plr.TPlayer.team = 2;
                TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: args.Who);
                plr.TPlayer.hostile = true;
                TSPlayer.All.SendData(PacketTypes.TogglePvp, number: args.Who);
            }
        }

        #endregion
    }
}