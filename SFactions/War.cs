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
            if (Commands.ActiveWar == null) return;
            
            TSPlayer.All.SendInfoMessage($"{Commands.ActiveWar.Faction1.Name} vs {Commands.ActiveWar.Faction2.Name} war has started!");

            foreach (TSPlayer p in TShock.Players)
            {
                if (p != null && p.Active && SFactions.OnlineMembers.ContainsKey((byte)p.Index))
                {
                    if (SFactions.OnlineMembers[(byte)p.Index] == Commands.ActiveWar.Faction1.Id)
                    {
                        p.TPlayer.team = 1;
                        TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: p.Index);
                        p.TPlayer.hostile = true;
                        TSPlayer.All.SendData(PacketTypes.TogglePvp, number: p.Index);
                    }
                    else if (SFactions.OnlineMembers[(byte)p.Index] == Commands.ActiveWar.Faction2.Id)
                    {
                        p.TPlayer.team = 2;
                        TSPlayer.All.SendData(PacketTypes.PlayerTeam, number: p.Index);
                        p.TPlayer.hostile = true;
                        TSPlayer.All.SendData(PacketTypes.TogglePvp, number: p.Index);
                    }
                }
            }

            ServerApi.Hooks.NetGreetPlayer.Register(SFactions.Instance, Handlers.OnNetGreetPlayer_War);
            GetDataHandlers.KillMe += Handlers.OnKillMe_War;
            GetDataHandlers.PlayerTeam += Handlers.OnPlayerChangeTeam_War;
            GetDataHandlers.TogglePvp += Handlers.OnPlayerTogglePvP_War;

            await Task.Delay(1 * 60 * 1000);
            TSPlayer.All.SendInfoMessage($"{Commands.ActiveWar.Faction1.Name}: {Commands.ActiveWar.KillCount1} vs {Commands.ActiveWar.Faction2.Name}: {Commands.ActiveWar.KillCount2}");

            await Task.Delay(1 * 60 * 1000);
            TSPlayer.All.SendInfoMessage($"{Commands.ActiveWar.Faction1.Name}: {Commands.ActiveWar.KillCount1} vs {Commands.ActiveWar.Faction2.Name}: {Commands.ActiveWar.KillCount2}");

            await Task.Delay(1 * 60 * 1000);
            TSPlayer.All.SendInfoMessage($"{Commands.ActiveWar.Faction1.Name}: {Commands.ActiveWar.KillCount1} vs {Commands.ActiveWar.Faction2.Name}: {Commands.ActiveWar.KillCount2}");


            if (Commands.ActiveWar.KillCount1 == Commands.ActiveWar.KillCount2)
            {
                TSPlayer.All.SendSuccessMessage($"The war between {Commands.ActiveWar.Faction1.Name} and {Commands.ActiveWar.Faction2.Name} ended as a TIE!");
            }
            else if (Commands.ActiveWar.KillCount1 > Commands.ActiveWar.KillCount2)
            {
                TSPlayer.All.SendSuccessMessage($"The war between {Commands.ActiveWar.Faction1.Name} and {Commands.ActiveWar.Faction2.Name} ended as {Commands.ActiveWar.Faction1.Name} being victorious!");

                foreach (var kvp in SFactions.OnlineMembers)
                {
                    if (kvp.Value == Commands.ActiveWar.Faction1.Id)
                    {
                        TSPlayer p = TShock.Players[kvp.Key];
                        string cmd = SFactions.Config.FactionWarWinCommand.Replace("%playername%", p.Name);
                        TShockAPI.Commands.HandleCommand(TSPlayer.Server, cmd);
                    }
                }
            }
            else
            {
                TSPlayer.All.SendSuccessMessage($"The war between {Commands.ActiveWar.Faction1.Name} and {Commands.ActiveWar.Faction2.Name} ended as {Commands.ActiveWar.Faction2.Name} being victorious!");
            }

            ServerApi.Hooks.NetGreetPlayer.Deregister(SFactions.Instance, Handlers.OnNetGreetPlayer_War);
            GetDataHandlers.KillMe -= Handlers.OnKillMe_War;
            GetDataHandlers.PlayerTeam -= Handlers.OnPlayerChangeTeam_War;
            GetDataHandlers.TogglePvp -= Handlers.OnPlayerTogglePvP_War;

            Commands.ActiveWar = null;
        }
    }
}