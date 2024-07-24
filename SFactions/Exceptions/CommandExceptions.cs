namespace SFactions
{
    public class GenericCommandException : Exception
    {
        public string ErrorMessage { get; set; }

        public GenericCommandException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }


    public class PlayerNotInFactionCommandException : GenericCommandException
    {
        public PlayerNotInFactionCommandException(string errorMessage = "You're not in a faction.") : base(errorMessage) { }

    }

    public class PlayerNotLeaderCommandException : GenericCommandException
    {
        public PlayerNotLeaderCommandException(string errorMessage = "Only leaders can use this command.") : base(errorMessage) { }

    }

    public class MissingArgumentCommandException : GenericCommandException
    {
        public MissingArgumentCommandException(string errorMessage = "Missing an argument.") : base(errorMessage) { }
    }

    public class FactionNotFoundCommandException : GenericCommandException
    {
        public FactionNotFoundCommandException(string errorMessage = "Faction not found.") : base(errorMessage) { }
    }

    public class PlayerNotFoundCommandException : GenericCommandException
    {
        public PlayerNotFoundCommandException(string errorMessage = "Player not found.") : base(errorMessage) { }
    }
}