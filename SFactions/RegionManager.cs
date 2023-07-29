using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using SFactions.Database;

namespace SFactions
{
    public class RegionManager{
        public static void SetRegion(TSPlayer plr) {
            SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]].Region = plr.CurrentRegion.Name;
            SFactions.dbManager.SaveFaction(SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]]);
        }

        public static void DelRegion(TSPlayer plr) {
            SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]].Region = null;
            SFactions.dbManager.SaveFaction(SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]]);
        }

        public static void AddMember(TSPlayer plr) {
            string? regionName = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]].Region;
            if (regionName != null) {
                TShock.Regions.AddNewUser(regionName, plr.Name);
            }
        }
        public static void DelMember(TSPlayer plr) {
            string? regionName = SFactions.onlineFactions[SFactions.onlineMembers[(byte)plr.Index]].Region;
            if (regionName != null) {
                TShock.Regions.RemoveUser(regionName, plr.Name);
            }
        }

        public static void AddAllMembers(Faction faction) {
            List<string> memberNames = SFactions.dbManager.GetAllMembers(faction.Id);
            if (faction.Region == null) {
                throw new NullReferenceException();
            }

            foreach (string memberName in memberNames) {
                TShock.Regions.AddNewUser(faction.Name, memberName);
            }
        }

        public static void DelAllMembers(Faction faction) {
            List<string> memberNames = SFactions.dbManager.GetAllMembers(faction.Id);
            if (faction.Region == null) {
                throw new NullReferenceException();
            }

            foreach (string memberName in memberNames) {
                TShock.Regions.RemoveUser(faction.Region, memberName);
            }
        }
    }
}
