using Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFactions.Database {
    public class Faction {

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Leader { get; set; }
        public AbilityType Ability { get; set; }
        public string? Region { get; set; }

        public Faction(int id, string name, string leader, AbilityType ability, string region) {
            Id = id;
            Name = name;
            Leader = leader;
            Ability = ability;
            Region = region;
        }
    }
}
