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
        public int? BaseX { get; set; }
        public int? BaseY { get; set; }

        public Faction(int id, string name, string? leader, AbilityType abilityType, string? region,
            DateTime? lastAbilityChangeTime, InviteType inviteType = InviteType.Open, int? baseX = null, int? baseY = null)
        {
            Id = id;
            Name = name;
            Leader = leader;
            AbilityType = abilityType;
            Region = region;
            BaseX = baseX;
            BaseY = baseY;

            if (lastAbilityChangeTime == null)
            {
                LastAbilityChangeTime = DateTime.Now;
            }
            else
            {
                LastAbilityChangeTime = (DateTime)lastAbilityChangeTime;
            }

            InviteType = inviteType;
            Ability = Utils.CreateAbility(AbilityType, Utils.GetAbilityLevel(this));
        }
    }
}
