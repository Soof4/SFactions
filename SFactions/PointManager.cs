using Terraria;
using SFactions.Database;

namespace SFactions
{
    public class PointManager
    {
        public static int GetAbilityLevel(Faction faction)
        {
            int passedTimeSinceAbilityChange = (int)(DateTime.UtcNow - faction.LastAbilityChangeTime).TotalHours;
            int abilityChangePunishment = 4;
            int bossLevel = 1;


            if (NPC.downedGolemBoss)
            {
                bossLevel = 5;
            }
            else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                bossLevel = 4;
            }
            else if (Main.hardMode)    // Wall of Flesh
            {
                bossLevel = 3;
            }
            else if (NPC.downedBoss2)    // Evil bosses
            {
                bossLevel = 2;
            }

            if (passedTimeSinceAbilityChange > 12) {
                abilityChangePunishment = 0;
            }
            else if (passedTimeSinceAbilityChange > 6) {
                abilityChangePunishment = 1;
            }
            else if (passedTimeSinceAbilityChange > 3) {
                abilityChangePunishment = 2;
            }
            else if (passedTimeSinceAbilityChange > 1.5) {
                abilityChangePunishment = 3;
            }
            
            if (abilityChangePunishment >= bossLevel) {
                return 1;
            }
            
            return bossLevel - abilityChangePunishment;
        }

        public static int GetAbilityCooldownByAbilityLevel(int abilityLevel, int cooldown)
        {
            return cooldown - 10 * abilityLevel;
        }
    }
}
