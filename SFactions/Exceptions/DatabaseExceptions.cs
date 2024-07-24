namespace SFactions.Exceptions
{
    public class DatabaseException : Exception { }
    public class FactionDoesNotExistDatabaseException : DatabaseException { }
    public class MemberDoesNotExistDatabaseException : DatabaseException { }

}