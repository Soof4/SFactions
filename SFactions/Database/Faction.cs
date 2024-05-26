using System.Drawing;
using System.Reflection;
using Abilities;

namespace SFactions.Database
{
    public class Faction
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Leader { get; set; }
        public AbilityType AbilityType { get; set; }
        public string? Region { get; set; }
        public InviteType InviteType { get; set; }
        public Ability Ability { get; set; }
        public DateTime LastAbilityChangeTime { get; set; }

        public Faction(int id, string name, string leader, AbilityType abilityType, string? region,
            DateTime? lastAbilityChangeTime, InviteType inviteType = InviteType.Open)
        {
            Id = id;
            Name = name;
            Leader = leader;
            AbilityType = abilityType;
            Region = region;

            if (lastAbilityChangeTime == null)
            {
                LastAbilityChangeTime = DateTime.Now;
            }
            else
            {
                LastAbilityChangeTime = (DateTime)lastAbilityChangeTime;
            }

            InviteType = inviteType;
            Ability = InstantiateTheAbility(AbilityType, this);
        }

        private static Ability InstantiateTheAbility(AbilityType abilityType, Faction faction)
        {
            return abilityType switch
            {
                AbilityType.DryadsRingOfHealing => new DryadsRingOfHealing(PointManager.GetAbilityLevel(faction)),
                AbilityType.RingOfDracula => new RingOfDracula(PointManager.GetAbilityLevel(faction)),
                AbilityType.SetsBlessing => new SetsBlessing(PointManager.GetAbilityLevel(faction)),
                AbilityType.Adrenaline => new Adrenaline(PointManager.GetAbilityLevel(faction)),
                AbilityType.Witch => new Witch(PointManager.GetAbilityLevel(faction)),
                AbilityType.Marthymr => new Marthymr(PointManager.GetAbilityLevel(faction)),
                AbilityType.RandomTeleport => new RandomTeleport(PointManager.GetAbilityLevel(faction)),
                AbilityType.FairyOfLight => new FairyOfLight(PointManager.GetAbilityLevel(faction)),
                AbilityType.Twilight => new Twilight(PointManager.GetAbilityLevel(faction)),
                AbilityType.Harvest => new Harvest(PointManager.GetAbilityLevel(faction)),
                AbilityType.IceGolem => new IceGolem(PointManager.GetAbilityLevel(faction)),
                AbilityType.MagicDice => new MagicDice(PointManager.GetAbilityLevel(faction)),
                AbilityType.TheBound => new TheBound(PointManager.GetAbilityLevel(faction)),
                AbilityType.Alchemist => new Alchemist(PointManager.GetAbilityLevel(faction)),
                AbilityType.Paranoia => new Paranoia(PointManager.GetAbilityLevel(faction)),
                AbilityType.HyperCrit => new HyperCrit(PointManager.GetAbilityLevel(faction)),
                AbilityType.Pentagram => new Pentagram(PointManager.GetAbilityLevel(faction)),
                AbilityType.SilentOrchestra => new SilentOrchestra(PointManager.GetAbilityLevel(faction)),
                AbilityType.Shockwave => new Shockwave(PointManager.GetAbilityLevel(faction)),
                _ => new DryadsRingOfHealing(PointManager.GetAbilityLevel(faction))
            };
        }
    }
}
