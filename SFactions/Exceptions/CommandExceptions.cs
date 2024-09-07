using SFactions.i18net;

namespace SFactions.Exceptions
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
        public PlayerNotInFactionCommandException() : base(Localization.ErrorMessage_NotInFaction) { }
    }

    public class PlayerIsInFactionCommandException : GenericCommandException
    {
        public PlayerIsInFactionCommandException() : base(Localization.ErrorMessage_InFaction) { }
    }


    public class PlayerNotLeaderCommandException : GenericCommandException
    {
        public PlayerNotLeaderCommandException() : base(Localization.ErrorMessage_LeaderOnly) { }
    }

    public class MissingArgumentCommandException : GenericCommandException
    {
        public MissingArgumentCommandException() : base(Localization.ErrorMessage_MissingArgument) { }
    }

    public class FactionNotFoundCommandException : GenericCommandException
    {
        public FactionNotFoundCommandException() : base(Localization.ErrorMessage_FactionNotFound) { }
    }

    public class PlayerNotFoundCommandException : GenericCommandException
    {
        public PlayerNotFoundCommandException() : base(Localization.ErrorMessage_PlayerNotFound) { }
    }
}