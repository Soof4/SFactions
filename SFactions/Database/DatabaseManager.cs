using Abilities;
using MySql.Data.MySqlClient;
using System.Data;
using TShockAPI.DB;

namespace SFactions.Database
{
    public class DatabaseManager
    {
        private IDbConnection _db;

        public DatabaseManager(IDbConnection db)
        {
            _db = db;

            var sqlCreator = new SqlTableCreator(db, new SqliteQueryCreator());

            sqlCreator.EnsureTableStructure(new SqlTable("Factions",
                new SqlColumn("Id", MySqlDbType.Int32) { Primary = true, Unique = true, AutoIncrement = true },
                new SqlColumn("Name", MySqlDbType.TinyText) { Unique = true },
                new SqlColumn("Leader", MySqlDbType.TinyText),
                new SqlColumn("AbilityType", MySqlDbType.Int32),
                new SqlColumn("Region", MySqlDbType.TinyText),
                new SqlColumn("LastAbilityChangeTime", MySqlDbType.TinyText),
                new SqlColumn("InviteType", MySqlDbType.Int32),
                new SqlColumn("BaseX", MySqlDbType.Int32),
                new SqlColumn("BaseY", MySqlDbType.Int32)
                ));

            sqlCreator.EnsureTableStructure(new SqlTable("Members",
                new SqlColumn("Member", MySqlDbType.TinyText) { Primary = true, Unique = true },
                new SqlColumn("FactionId", MySqlDbType.Int32)
                ));
        }

        #region Faction Management
        public bool InsertFaction(string leaderName, string factionName)
        {
            Utils.TryGetAbilityTypeFromString(SFactions.Config.DefaultAbility, out AbilityType defAbilityType);
            return _db.Query("INSERT INTO Factions (Name, Leader, AbilityType, InviteType, LastAbilityChangeTime) " +
                "VALUES (@0, @1, @2, @3, @4)",
                factionName, leaderName, defAbilityType, InviteType.Open, DateTime.UtcNow.ToString()) != 0;
        }


        public bool SaveFaction(Faction faction)
        {
            return _db.Query("UPDATE Factions SET Name = @1, Leader = @2, AbilityType = @3, Region = @4, InviteType = @5, LastAbilityChangeTime = @6, BaseX = @7, BaseY = @8 WHERE Id = @0",
                faction.Id, faction.Name, faction.Leader, (int)faction.AbilityType, faction.Region, faction.InviteType, faction.LastAbilityChangeTime, faction.BaseX, faction.BaseY) != 0;
        }

        /// <summary>
        /// Checks if the faction with this name exist.
        /// </summary>
        public bool DoesFactionExist(string name)
        {
            using var reader = _db.QueryReader("SELECT * FROM Factions WHERE Name = @0", name);
            while (reader.Read())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the faction from database.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public Faction GetFaction(string factionName)
        {
            using var reader = _db.QueryReader("SELECT * FROM Factions WHERE Name = @0", factionName);
            while (reader.Read())
            {
                return new Faction(
                    reader.Get<int>("Id"),
                    reader.Get<string>("Name"),
                    reader.Get<string>("Leader"),
                    (AbilityType)reader.Get<int>("AbilityType"),
                    reader.Get<string>("Region"),
                    DateTime.Parse(reader.Get<string>("LastAbilityChangeTime")),
                    (InviteType)reader.Get<int>("InviteType"),
                    reader.Get<int?>("BaseX"),
                    reader.Get<int?>("BaseY")
                    );
            }
            throw new NullReferenceException();
        }

        /// <summary>
        /// Gets player's faction from databse.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public Faction GetPlayerFaction(string playerName)
        {
            using var reader = _db.QueryReader("SELECT * FROM Members WHERE Member = @0", playerName);
            while (reader.Read())
            {
                using var reader2 = _db.QueryReader("SELECT * FROM Factions WHERE Id = @0", reader.Get<int>("FactionId"));
                while (reader2.Read())
                {
                    return new Faction(
                    reader2.Get<int>("Id"),
                    reader2.Get<string>("Name"),
                    reader2.Get<string>("Leader"),
                    (AbilityType)reader2.Get<int>("AbilityType"),
                    reader2.Get<string>("Region"),
                    DateTime.Parse(reader2.Get<string>("LastAbilityChangeTime")),
                    (InviteType)reader2.Get<int>("InviteType"),
                    reader2.Get<int?>("BaseX"),
                    reader2.Get<int?>("BaseY")
                    );
                }
            }
            throw new NullReferenceException();
        }
        #endregion


        #region Member Management
        public bool InsertMember(string name, int factionId)
        {
            return _db.Query("INSERT INTO Members (Member, FactionId) VALUES (@0, @1)", name, factionId) != 0;
        }

        public bool DeleteMember(string name)
        {
            return _db.Query("DELETE FROM Members WHERE Member = @0", name) != 0;
        }

        public List<string> GetAllMembers(int factionId)
        {
            List<string> memberNames = new();
            using var reader = _db.QueryReader("SELECT * FROM Members WHERE FactionId = @0", factionId);
            while (reader.Read())
            {
                memberNames.Add(reader.Get<string>("Member"));
            }
            return memberNames;
        }
        #endregion
    }
}
