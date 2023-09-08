using Abilities;
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
                new SqlColumn("Leader", MySqlDbType.TinyText),
                new SqlColumn("AbilityType", MySqlDbType.Int32),
                new SqlColumn("Region", MySqlDbType.TinyText)
                /*,
                new SqlColumn("Point", MySqlDbType.Int32),
                new SqlColumn("baseQuestComplete", MySqlDbType.Int32),
                new SqlColumn("highestMemberCount", MySqlDbType.Int32)
                */
                ));

            sqlCreator.EnsureTableStructure(new SqlTable("Members",
                new SqlColumn("Member", MySqlDbType.TinyText) { Primary = true, Unique = true},
                new SqlColumn("FactionId", MySqlDbType.Int32)
                ));

            /*
            sqlCreator.EnsureTableStructure(new SqlTable("Quests",
                new SqlColumn("FactionId", MySqlDbType.Int32) { Primary = true, Unique = true},
                new SqlColumn("ItemId", MySqlDbType.Int32),
                new SqlColumn("Amount", MySqlDbType.Int32)
                ));
            */
        }

        #region Faction Management
        public bool InsertFaction(string leaderName, string factionName) {
            /*
            return _db.Query("INSERT INTO Factions (Name, Leader, AbilityType, Point, baseQuestComplete, highestMemberCount) " +
                "VALUES (@0, @1, @2, @3, @4, @5)", 
                factionName, leaderName, AbilityType.DryadsRingOfHealing, 0, 0, 1) != 0;
            */
            return _db.Query("INSERT INTO Factions (Name, Leader, AbilityType) " +
                "VALUES (@0, @1, @2)",
                factionName, leaderName, AbilityType.DryadsRingOfHealing) != 0;
        }


        public bool SaveFaction(Faction faction) {
            /*
            return _db.Query("UPDATE Factions SET Name = @1, Leader = @2, AbilityType = @3, Region = @4, " +
                "Point = @5, baseQuestComplete = @6, highestMemberCount = @7 WHERE Id = @0",
                faction.Id, faction.Name, faction.Leader, (int)faction.Ability, faction.Region,
                faction.Point, Utils.BoolToInt(faction.baseQuestComplete), faction.highestMemberCount) != 0;
            */
            return _db.Query("UPDATE Factions SET Name = @1, Leader = @2, AbilityType = @3, Region = @4 WHERE Id = @0",
                faction.Id, faction.Name, faction.Leader, (int)faction.Ability, faction.Region) != 0;
        }


        public bool DoesFactionExist(string name) {
            using var reader = _db.QueryReader("SELECT * FROM Factions WHERE Name = @0", name);
            while (reader.Read()) {
                return true;
            }
            return false;
        }


        /// <exception cref="NullReferenceException"></exception>
        public Faction GetFaction(string factionName) {
            using var reader = _db.QueryReader("SELECT * FROM Factions WHERE Name = @0", factionName);
            while (reader.Read()) {
                return new Faction(
                    reader.Get<int>("Id"),
                    reader.Get<string>("Name"),
                    reader.Get<string>("Leader"),
                    (AbilityType)reader.Get<int>("AbilityType"),
                    reader.Get<string>("Region")
                    /*,
                    reader.Get<int>("Point"),
                    Utils.IntToBool(reader.Get<int>("baseQuestComplete")),
                    reader.Get<int>("highestMemberCount")*/
                    );
            }
            throw new NullReferenceException();
        }


        /// <exception cref="NullReferenceException"></exception>
        public Faction GetPlayerFaction(string playerName) {
            using var reader = _db.QueryReader("SELECT * FROM Members WHERE Member = @0", playerName);
            while (reader.Read()) {
                using var reader2 = _db.QueryReader("SELECT * FROM Factions WHERE Id = @0", reader.Get<int>("FactionId"));
                while (reader2.Read()) {
                    return new Faction(
                    reader2.Get<int>("Id"),
                    reader2.Get<string>("Name"),
                    reader2.Get<string>("Leader"),
                    (AbilityType)reader2.Get<int>("AbilityType"),
                    reader2.Get<string>("Region")
                    /*,
                    reader2.Get<int>("Point"),
                    Utils.IntToBool(reader2.Get<int>("baseQuestComplete")),
                    reader2.Get<int>("highestMemberCount")*/
                    );
                }
            }
            throw new NullReferenceException();
        }
        #endregion


        #region Member Management
        public bool InsertMember(string name, int factionId) {
            return _db.Query("INSERT INTO Members (Member, FactionId) VALUES (@0, @1)", name, factionId) != 0;
        }

        public bool DeleteMember(string name) {
            return _db.Query("DELETE FROM Members WHERE Member = @0", name) != 0;
        }

        public List<string> GetAllMembers(int factionId) {
            List<string> memberNames = new();
            using var reader = _db.QueryReader("SELECT * FROM Members WHERE FactionId = @0", factionId);
            while (reader.Read()) {
                memberNames.Add(reader.Get<string>("Member"));
            }
            return memberNames;
        }
        #endregion

        /*
        #region Quest Management
        public bool InsertQuest(int factionId, int itemId, int amount) {
            return _db.Query("INSERT INTO Quests (FactionId, ItemId, Amount) VALUES (@0, @1, @2)", factionId, itemId, amount) != 0;
        }


        public bool SaveQuest(int factionId, int itemId, int amount) {
            return _db.Query("UPDATE Quests SET ItemId = @0, Amount = @1 WHERE FactionId = @2", itemId, amount, factionId) != 0;
        }


        public bool DeleteQuest(int factionId) {
            return _db.Query("DELETE FROM Quests WHERE FactionId = @0", factionId) != 0;
        }


        /// <exception cref="NullReferenceException"></exception>
        /// <returns>quest[0] = ItemId<br>
        /// </br>quest[1] = Amount</returns>
        public int[] GetQuest(int factionId) {
            using var reader = _db.QueryReader("SELECT * FROM Quests WHERE FactionId = @0", factionId);
            while (reader.Read()) {
                int[] result = { reader.Get<int>("ItemId"), reader.Get<int>("Amount") };
                return result;
            }
            throw new NullReferenceException();
        }

        #endregion
        */
    }
}
