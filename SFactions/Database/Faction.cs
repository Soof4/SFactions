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

        /*
        public int Point { get; set; }
        public bool baseQuestComplete { get; set; }
        public int highestMemberCount { get; set; }
        */

        public Faction(int id, string name, string leader, AbilityType ability, string region/*, int point, bool baseQuestComplete, int highestMemberCount*/) {
            Id = id;
            Name = name;
            Leader = leader;
            Ability = ability;
            Region = region;
            /*
            Point = point;
            this.baseQuestComplete = false;
            this.highestMemberCount = 1;
            */
        }
    }
}
