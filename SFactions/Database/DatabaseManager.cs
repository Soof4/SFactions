using Abilities;
using MySql.Data.MySqlClient;
using SFactions.Exceptions;
using System.Data;
using TShockAPI.DB;
using Abilities.Enums;
using Abilities.Abilities;

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
        public async Task<bool> InsertFactionAsync(string leaderName, string factionName)
        {
            string q = "INSERT INTO Factions (Name, Leader, AbilityType, InviteType, LastAbilityChangeTime) VALUES (@0, @1, @2, @3, @4)";

            Utils.TryGetAbilityTypeFromString(SFactions.Config.DefaultAbility, out AbilityType defAbilityType);
            int num = await _db.AsyncQuery(q, factionName, leaderName, defAbilityType, InviteType.Open, DateTime.UtcNow.ToString());

            return num > 0;
        }

        public async Task<bool> SaveFactionAsync(Faction faction)
        {

            string q = "UPDATE Factions SET Name = @1, Leader = @2, AbilityType = @3, Region = @4, InviteType = @5, LastAbilityChangeTime = @6, BaseX = @7, BaseY = @8 WHERE Id = @0";
            int num = await _db.AsyncQuery(q, faction.Id, faction.Name, faction.Leader, (int)faction.AbilityType, faction.Region, faction.InviteType, faction.LastAbilityChangeTime, faction.BaseX, faction.BaseY);

            return num > 0;

        }

        public async Task<bool> DoesFactionExistAsync(string name)
        {
            string q = "SELECT * FROM Factions WHERE Name = @0";

            using var reader = await _db.AsyncQueryReader(q, name);
            while (reader.Read())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factionName"></param>
        /// <returns></returns>
        /// <exception cref="FactionDoesNotExistDatabaseException"></exception>
        public async Task<Faction> GetFactionAsync(string factionName)
        {
            string q = "SELECT * FROM Factions WHERE Name = @0";

            using var reader = await _db.AsyncQueryReader(q, factionName);
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
            throw new FactionDoesNotExistDatabaseException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        /// <exception cref="FactionDoesNotExistDatabaseException"></exception>
        public async Task<Faction> GetPlayerFactionAsync(string playerName)
        {
            string q = "SELECT * FROM Members WHERE Member = @0";

            using var reader = await _db.AsyncQueryReader(q, playerName);
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
            throw new FactionDoesNotExistDatabaseException();
        }
        #endregion


        #region Member Management
        public async Task<bool> InsertMemberAsync(string name, int factionId)
        {
            string q = "INSERT INTO Members (Member, FactionId) VALUES (@0, @1)";
            int num = await _db.AsyncQuery(q, name, factionId);

            return num > 0;
        }

        public async Task<bool> DeleteMemberAsync(string name)
        {
            string q = "DELETE FROM Members WHERE Member = @0";
            int num = await _db.AsyncQuery(q, name);

            return num > 0;
        }


        public async Task<List<string>> GetAllMembersAsync(int factionId)
        {
            string q = "SELECT * FROM Members WHERE FactionId = @0";
            List<string> memberNames = new List<string>();

            using var reader = await _db.AsyncQueryReader(q, factionId);
            while (reader.Read())
            {
                memberNames.Add(reader.Get<string>("Member"));
            }

            return memberNames;
        }

        #endregion
    }
}
