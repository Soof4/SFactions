using Terraria;

namespace SFactions {
    public class PointManager {
        public static int GetLevelByPoint(int point) {
            if (point < 5) {
                return 1;
            }
            else if (point < 11) {
                return 2;
            }
            else if (point < 19) {
                return 3;
            }
            else if (point < 30) {
                return 4;
            }
            else {
                return 5;
            }
        }
        
        public static int GetAbilityLevelByBossProgression() {
            if (NPC.downedGolemBoss) { return 5; }    // golem
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3) { return 4; }    // all mechs
            if (Main.hardMode) { return 3; }    // wof
            if (NPC.downedBoss2) { return 2; }    // evil bosses
            return 1;
        }
    }
}
