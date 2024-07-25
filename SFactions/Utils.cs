using Abilities;
using SFactions.Database;
using TShockAPI;
using Terraria;
using System.Text;
using System.Data;
using TShockAPI.DB;

namespace SFactions
{
    public static class Utils
    {
        public static Ability CreateAbility(AbilityType abilityType, int abilityLevel)
        {
            return abilityType switch
            {
                AbilityType.DryadsRingOfHealing => new DryadsRingOfHealing(abilityLevel),
                AbilityType.RingOfDracula => new RingOfDracula(abilityLevel),
                AbilityType.SetsBlessing => new SetsBlessing(abilityLevel),
                AbilityType.Adrenaline => new Adrenaline(abilityLevel),
                AbilityType.Witch => new Witch(abilityLevel),
                AbilityType.Marthymr => new Marthymr(abilityLevel),
                AbilityType.RandomTeleport => new RandomTeleport(abilityLevel),
                AbilityType.FairyOfLight => new FairyOfLight(abilityLevel),
                AbilityType.Twilight => new Twilight(abilityLevel),
                AbilityType.Harvest => new Harvest(abilityLevel),
                AbilityType.IceGolem => new IceGolem(abilityLevel),
                AbilityType.MagicDice => new MagicDice(abilityLevel),
                AbilityType.TheBound => new TheBound(abilityLevel),
                AbilityType.Alchemist => new Alchemist(abilityLevel),
                AbilityType.Paranoia => new Paranoia(abilityLevel),
                AbilityType.HyperCrit => new HyperCrit(abilityLevel),
                AbilityType.Pentagram => new Pentagram(abilityLevel),
                AbilityType.SilentOrchestra => new SilentOrchestra(abilityLevel),
                AbilityType.Shockwave => new Shockwave(abilityLevel),
                _ => new DryadsRingOfHealing(abilityLevel)
            };
        }

        public static bool TryGetAbilityTypeFromString(string type, out AbilityType result)
        {
            switch (type.ToLower())
            {
                case "dryadsringofhealing":
                    result = AbilityType.DryadsRingOfHealing; break;
                case "ringofdracula":
                    result = AbilityType.RingOfDracula; break;
                case "setsblessing":
                    result = AbilityType.SetsBlessing; break;
                case "adrenaline":
                    result = AbilityType.Adrenaline; break;
                case "witch":
                    result = AbilityType.Witch; break;
                case "marthymr":
                    result = AbilityType.Marthymr; break;
                case "randomtp":
                    result = AbilityType.RandomTeleport; break;
                case "fairyoflight":
                    result = AbilityType.FairyOfLight; break;
                case "twilight":
                    result = AbilityType.Twilight; break;
                case "harvest":
                    result = AbilityType.Harvest; break;
                case "icegolem":
                    result = AbilityType.IceGolem; break;
                case "magicdice":
                    result = AbilityType.MagicDice; break;
                case "thebound":
                    result = AbilityType.TheBound; break;
                case "alchemist":
                    result = AbilityType.Alchemist; break;
                case "paranoia":
                    result = AbilityType.Paranoia; break;
                case "hypercrit":
                    result = AbilityType.HyperCrit; break;
                case "pentagram":
                    result = AbilityType.Pentagram; break;
                case "silentorchestra":
                    result = AbilityType.SilentOrchestra; break;
                case "shockwave":
                    result = AbilityType.Shockwave; break;
                default:
                    result = AbilityType.DryadsRingOfHealing;
                    return false;
            }

            return true;
        }

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

            if (passedTimeSinceAbilityChange > 12)
            {
                abilityChangePunishment = 0;
            }
            else if (passedTimeSinceAbilityChange > 6)
            {
                abilityChangePunishment = 1;
            }
            else if (passedTimeSinceAbilityChange > 3)
            {
                abilityChangePunishment = 2;
            }
            else if (passedTimeSinceAbilityChange > 1.5)
            {
                abilityChangePunishment = 3;
            }

            if (abilityChangePunishment >= bossLevel)
            {
                return 1;
            }

            return bossLevel - abilityChangePunishment;
        }

        public static int GetAbilityCooldownByAbilityLevel(int abilityLevel, int cooldown)
        {
            return cooldown - 10 * abilityLevel;
        }

        public static string ToTitleCase(string str)
        {
            if (str.Trim().Length <= 1) return str.Trim().ToUpper();

            str = str.Trim();

            StringBuilder builder = new();
            builder.Append(str[0]);

            char preC = str[0];
            char c = str[1];

            for (int i = 1; i < str.Length; i++, preC = c)
            {
                c = str[i];
                if (char.IsWhiteSpace(preC) && char.IsWhiteSpace(c))
                {
                    continue;
                }

                if (char.IsDigit(c))
                {
                    if (!char.IsDigit(preC) && !char.IsWhiteSpace(preC)) builder.Append(' ');

                    builder.Append(c);

                }
                else if (char.IsUpper(c))
                {
                    if (!char.IsWhiteSpace(preC)) builder.Append(' ');

                    builder.Append(c);
                }
                else    // If c is lowercase
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static string GetFirstWord(string str)
        {
            if (str.Trim().Length <= 1) return str.Trim().ToUpper();

            str = str.Trim();

            string res = str[0].ToString().ToLower();

            for (int i = 1; i < str.Length; i++)
            {
                char c = str[i];

                if (char.IsUpper(c) || char.IsDigit(c))
                {
                    break;
                }

                res += c;
            }

            return res;
        }

        public static bool TryParseInt(string? s, ref int result)
        {
            try
            {
                result = int.Parse(s!);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static object _locker = new();
        public static Task<int> AsyncQuery(this IDbConnection db, string query, params object?[] args)
        {
            return Task.Run(() =>
            {
                lock (_locker)
                {
                    return db.Query(query, args);

                }
            });
        }

        public static Task<QueryResult> AsyncQueryReader(this IDbConnection db, string query, params object?[] args)
        {
            return Task.Run(() =>
            {
                lock (_locker)
                {
                    return db.QueryReader(query, args);
                }
            });
        }

        public static void Console_WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
