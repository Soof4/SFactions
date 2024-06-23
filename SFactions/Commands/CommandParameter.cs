namespace SFactions
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CommandParameter : Attribute
    {
        public string Name { get; }

        public CommandParameter(string name)
        {
            Name = name;
        }
    }
}