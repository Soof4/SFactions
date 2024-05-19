using Abilities;
using IL.Terraria.DataStructures;
using TShockAPI;

namespace SFactions
{
    public static class Utils
    {
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
                default:
                    result = AbilityType.DryadsRingOfHealing;
                    return false;
            }

            return true;
        }
    }
}
