namespace CG.Console
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public sealed class ConsoleCmdAttribute : System.Attribute
    {
        public string CommandName { get; }
        public string Description { get; }
        public bool HideFromAutoComplete { get; }
        public bool HideFromHelpCommand { get; }

        public ConsoleCmdAttribute(string commandName, string description = "", bool hideFromAutoComplete = false,
        bool hideFromHelpCommand = false)
        {
            CommandName = commandName;
            Description = description;
            HideFromAutoComplete = hideFromAutoComplete;
            HideFromHelpCommand = hideFromHelpCommand;
        }
    }
}