using TShockAPI;
using SFactions.Database;

namespace SFactions {
    public static class RegionManager {
        public static void SetRegion(TSPlayer plr) {
            Faction faction = OnlineFactions.GetPlayerFaction(plr.Index);
            faction.Region = plr.CurrentRegion.Name;
            faction.BaseX = plr.TileX;
            faction.BaseY = plr.TileY;
            SFactions.DbManager.SaveFaction(faction);
        }

        public static void DelRegion(TSPlayer plr) {
            Faction faction = OnlineFactions.GetPlayerFaction(plr.Index);
            faction.Region = null;
            faction.BaseX = null;
            faction.BaseY = null;
            SFactions.DbManager.SaveFaction(faction);
        }

        public static void AddMember(TSPlayer plr) {
            string? regionName = OnlineFactions.GetPlayerFaction(plr.Index).Region;
            if (regionName != null) {
                TShock.Regions.AddNewUser(regionName, plr.Name);
            }
        }
        public static void DelMember(TSPlayer plr) {
            string? regionName = OnlineFactions.GetPlayerFaction(plr.Index).Region;
            if (regionName != null) {
                TShock.Regions.RemoveUser(regionName, plr.Name);
            }
        }

        public static void AddAllMembers(Faction faction) {
            List<string> memberNames = SFactions.DbManager.GetAllMembers(faction.Id);
            if (faction.Region == null) {
                throw new NullReferenceException();
            }

            foreach (string memberName in memberNames) {
                TShock.Regions.AddNewUser(faction.Region, memberName);
            }
        }

        public static void DelAllMembers(Faction faction) {
            List<string> memberNames = SFactions.DbManager.GetAllMembers(faction.Id);
            if (faction.Region == null) {
                throw new NullReferenceException();
            }

            foreach (string memberName in memberNames) {
                TShock.Regions.RemoveUser(faction.Region, memberName);
            }
        }
    }
}
