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

        public Faction(int id, string name, string leader) {
            Id = id;
            Name = name;
            Leader = leader;
        }

        public bool IsLeader(string name) {
            return Leader != null && Leader.Equals(name);
        }
    }
}
