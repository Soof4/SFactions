using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL.Terraria.DataStructures;
using TShockAPI;
using Terraria.ID;

namespace SFactions {
    public class PointManager {
        /*
        public static Dictionary<int, DateTime> QuestChangeTimes = new();
        static readonly int minSize = 600;
        static readonly int maxSize = 2500;

        private class FTile {
            public int X;
            public int Y;

            public FTile(int x, int y) {
                X = x;
                Y = y;
            }

            public override bool Equals(object? obj) {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                FTile otherTile = (FTile)obj;
                return X == otherTile.X && Y == otherTile.Y;
            }

            public override int GetHashCode() {
                unchecked {
                    int hash = X;
                    hash <<= 16;
                    hash |= Y;
                    return hash;
                }
            }
        }

        /// <summary>
            /// Checks if there is a faction base at the position.
            /// </summary>
            /// <param name="x">Tile x coordinate.</param>
            /// <param name="y">Tile y coordinate.</param>
        public static bool CheckForFactionBase(TSPlayer plr) {

            if (!TShock.Regions.InAreaRegion(plr.TileX, plr.TileY).Any(region => region.Owner.Equals(plr.Name))) {
                plr.SendErrorMessage("This region is not protected by you.");
                return false;
            }

            List<FTile> preWave = new();
            List<FTile> curWave = new();
            List<FTile> newWave = new();

            curWave.Add(new(plr.TileX, plr.TileY));
            int size = 1;
            bool hasWarTable = false;

            while (true) {
                foreach (var tile in curWave) {
                    for (int i = -1; i < 2; i++) {
                        for (int j = -1; j < 2; j++) {                            
                            if (i == j || Math.Abs(i - j) == 2) {
                                continue;
                            }

                            FTile cTile = new(tile.X + i, tile.Y + j);

                            if (cTile.X < 0 || cTile.Y < 0 || cTile.X > Main.maxTilesX || cTile.Y > Main.maxTilesY) {
                                plr.SendErrorMessage("This base reaches the world boundry.");
                                return false;    // too big
                            }
                            
                            if (Main.tile[cTile.X, cTile.Y].collisionType != 1 && 
                                Main.tile[cTile.X, cTile.Y].type != TileID.OpenDoor && 
                                Main.tile[cTile.X, cTile.Y].type != TileID.Platforms &&
                                Main.tile[cTile.X, cTile.Y].type != TileID.TrapdoorOpen &&
                                Main.tile[cTile.X, cTile.Y].type != TileID.TallGateOpen &&
                                !preWave.Contains(cTile) &&
                                !newWave.Contains(cTile)) {
                                
                                if (Main.tile[cTile.X, cTile.Y].type == TileID.WarTable) {
                                    hasWarTable = true;
                                }

                                if (Main.tile[cTile.X, cTile.Y].wall == 0) {
                                    plr.SendErrorMessage("This base is missing a wall.");
                                    return false;    // missing wall
                                }

                                newWave.Add(cTile);
                                size++;
                            }
                        }
                    }
                }

                if (size > maxSize) {
                    plr.SendErrorMessage("This base is too big.");
                    return false;    // too big
                }

                if (newWave.Count == 0) {
                    if (size < minSize) {
                        plr.SendErrorMessage("This base is too small.");
                        return false;    // too small
                    }
                    else if (!hasWarTable) {
                        plr.SendErrorMessage("This base is missing a war table ([i:3814]).");
                        return false;    // 
                    }
                    else {
                        plr.SendSuccessMessage("This base met all the conditions!");
                        return true;    // all conditions met
                    }
                }

                preWave = curWave;
                curWave = newWave;
                newWave = new();
            }
        }

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
        */
        public static int GetAbilityLevelByBossProgression() {
            if (NPC.downedGolemBoss) { return 5; }    // golem
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3) { return 4; }    // all mechs
            if (Main.hardMode) { return 3; }    // wof
            if (NPC.downedBoss2) { return 2; }    // evil bosses
            return 1;
        }
    }
}
