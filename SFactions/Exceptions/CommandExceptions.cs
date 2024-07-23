namespace SFactions
{
    public class CommandException : Exception
    {
        public string ErrorMessage { get; set; }

        public CommandException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }


    public class PlayerNotInFactionException : CommandException
    {
        public PlayerNotInFactionException(string errorMessage = "You're not in a faction.") : base(errorMessage) { }

    }

    public class PlayerNotLeaderException : CommandException
    {
        public PlayerNotLeaderException(string errorMessage = "Only leaders can use this command.") : base(errorMessage) { }

    }

    public class MissingArgumentException : CommandException
    {
        public MissingArgumentException(string errorMessage = "Missing an argument.") : base(errorMessage) { }
    }

    public class FactionNotFoundException : CommandException
    {
        public FactionNotFoundException(string errorMessage = "Faction not found.") : base(errorMessage) { }
    }
}