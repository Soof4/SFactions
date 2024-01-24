using Terraria;

namespace SFactions {
    public class PointManager {
        public static int GetAbilityLevelByBossProgression() {
            if (NPC.downedGolemBoss) { return 5; }    // golem
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3) { return 4; }    // all mechs
            if (Main.hardMode) { return 3; }    // wof
            if (NPC.downedBoss2) { return 2; }    // evil bosses
            return 1;
        }

        public static int GetAbilityCooldownByAbilityLevel(int abilityLevel, int cooldown) {
            return cooldown - 10 * abilityLevel;
        }
    }
}
