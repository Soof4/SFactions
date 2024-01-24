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

        public Faction(int id, string name, string leader, AbilityType abilityType, string? region, InviteType inviteType = InviteType.Open)
        {
            Id = id;
            Name = name;
            Leader = leader;
            AbilityType = abilityType;
            Region = region;
            InviteType = inviteType;
            Ability = InstantiateTheAbility(AbilityType);
        }

        private static Ability InstantiateTheAbility(AbilityType abilityType)
        {
            return abilityType switch
            {
                AbilityType.DryadsRingOfHealing => new DryadsRingOfHealing(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.RingOfDracula => new RingOfDracula(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.SetsBlessing => new SetsBlessing(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.Adrenaline => new Adrenaline(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.Witch => new Witch(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.Marthymr => new Marthymr(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.RandomTeleport => new RandomTeleport(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.FairyOfLight => new FairyOfLight(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.Twilight => new Twilight(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.Harvest => new Harvest(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.IceGolem => new IceGolem(PointManager.GetAbilityLevelByBossProgression()),
                AbilityType.MagicDice => new MagicDice(PointManager.GetAbilityLevelByBossProgression()),
                _ => new DryadsRingOfHealing(PointManager.GetAbilityLevelByBossProgression()),
            };
        }
    }
}
