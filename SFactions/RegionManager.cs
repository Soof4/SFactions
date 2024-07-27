using TShockAPI;
using SFactions.Database;

namespace SFactions
{
    public static class RegionManager
    {
        public static void SetRegion(TSPlayer plr)
        {
            Faction faction = FactionService.GetPlayerFaction(plr.Index);
            faction.Region = plr.CurrentRegion.Name;
            faction.BaseX = plr.TileX;
            faction.BaseY = plr.TileY;
            _ = SFactions.DbManager.SaveFactionAsync(faction);
        }

        public static void DelRegion(TSPlayer plr)
        {
            Faction faction = FactionService.GetPlayerFaction(plr.Index);
            faction.Region = null;
            faction.BaseX = null;
            faction.BaseY = null;
            _ = SFactions.DbManager.SaveFactionAsync(faction);
        }

        public static void AddMember(TSPlayer plr)
        {
            string? regionName = FactionService.GetPlayerFaction(plr.Index).Region;
            if (regionName != null)
            {
                TShock.Regions.AddNewUser(regionName, plr.Name);
            }
        }
        public static void DelMember(TSPlayer plr)
        {
            string? regionName = FactionService.GetPlayerFaction(plr.Index).Region;
            if (regionName != null)
            {
                TShock.Regions.RemoveUser(regionName, plr.Name);
            }
        }

        public static async void AddAllMembers(Faction faction)
        {
            try
            {
                List<string> memberNames = await SFactions.DbManager.GetAllMembersAsync(faction.Id);
                if (faction.Region == null)
                {
                    throw new NullReferenceException();
                }

                foreach (string memberName in memberNames)
                {
                    TShock.Regions.AddNewUser(faction.Region, memberName);
                }
            }
            catch { }
        }

        public static async void DelAllMembers(Faction faction)
        {
            try
            {
                List<string> memberNames = await SFactions.DbManager.GetAllMembersAsync(faction.Id);
                if (faction.Region == null)
                {
                    throw new NullReferenceException();
                }

                foreach (string memberName in memberNames)
                {
                    TShock.Regions.RemoveUser(faction.Region, memberName);
                }
            }
            catch { }
        }
    }
}
