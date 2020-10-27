namespace MariCommands.Filters
{
    public class CommandResultContext
    {
        public CommandResultContext(CommandContext commandContext)
        {
            CommandContext = commandContext;
        }

        public virtual CommandContext CommandContext { get; }

        public virtual bool Cancel { get; set; }
    }
}