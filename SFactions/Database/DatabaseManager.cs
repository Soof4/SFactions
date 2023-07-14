using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using TShockAPI.DB;

namespace SFactions.Database
{
    public class DatabaseManager
    {
        private IDbConnection _db;

        public DatabaseManager(IDbConnection db) {
            _db = db;

            var sqlCreator = new SqlTableCreator(db, new SqliteQueryCreator());

            sqlCreator.EnsureTableStructure(new SqlTable("Factions",
                new SqlColumn("Id", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true },
                new SqlColumn("Name", MySqlDbType.TinyText) { Unique = true },
                new SqlColumn("Leader", MySqlDbType.TinyText)
                ));

            sqlCreator.EnsureTableStructure(new SqlTable("Members",
                new SqlColumn("Member", MySqlDbType.TinyText) { Primary = true, Unique = true},
                new SqlColumn("FactionId", MySqlDbType.Int32)
                ));
        }

        #region Faction Management
        public bool InsertFaction(string leaderName, string factionName) {
            return _db.Query("INSERT INTO Factions (Name, Leader) VALUES (@0, @1)", factionName, leaderName) != 0;
        }
        public bool SaveFaction(Faction faction) {
            return _db.Query("UPDATE Factions SET Name = @0, Leader = @1 WHERE Id = @2", faction.Name, faction.Leader, faction.Id) != 0;
        }

        /// <exception cref="NullReferenceException"></exception>
        public Faction GetFaction(int id) {
            using var reader = _db.QueryReader("SELECT * FROM Factions WHERE Id = @0", id);
            while (reader.Read()) {
                return new Faction(
                    reader.Get<int>("Id"),
                    reader.Get<string>("Name"),
                    reader.Get<string>("Leader"));
            }
            throw new NullReferenceException();
        }

        /// <exception cref="NullReferenceException"></exception>
        public Faction GetFaction(string name) {
            using var reader = _db.QueryReader("SELECT * FROM Factions WHERE Name = @0", name);
            while (reader.Read()) {
                return new Faction(
                    reader.Get<int>("Id"),
                    reader.Get<string>("Name"),
                    reader.Get<string>("Leader"));
            }
            throw new NullReferenceException();
        }

        /// <exception cref="NullReferenceException"></exception>
        public int GetPlayerFactionId(string name) {
            using var reader = _db.QueryReader("SELECT * FROM Members WHERE Member = @0", name);
            while (reader.Read()) {
                return reader.Get<int>("FactionId");

            }
            throw new NullReferenceException();
        }

        /// <exception cref="NullReferenceException"></exception>
        public Faction GetPlayerFaction(string name) {
            using var reader = _db.QueryReader("SELECT * FROM Members WHERE Member = @0", name);
            while (reader.Read()) {
                using var reader2 = _db.QueryReader("SELECT * FROM Factions WHERE Id = @0", reader.Get<int>("FactionId"));
                while (reader2.Read()) {
                    return new Faction(
                    reader2.Get<int>("Id"),
                    reader2.Get<string>("Name"),
                    reader2.Get<string>("Leader"));
                }
            }
            throw new NullReferenceException();
        }
        #endregion

        #region Member Management
        public bool InsertMember(string name, int factionId) {
            return _db.Query("INSERT INTO Members (Member, FactionId) VALUES (@0, @1)", name, factionId) != 0;
        }

        public bool SaveMember(string name, int factionId) {
            return _db.Query("UPDATE Members SET FactionId = @0 WHERE Member = @1", factionId, name) != 0;
        }

        public bool DeleteMember(string name) {
            return _db.Query("DELETE FROM Members WHERE Member = @0", name) != 0;
        }

        
        #endregion
    }
}
