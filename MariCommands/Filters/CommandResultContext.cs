namespace MariCommands.Filters
{
    /// <summary>
    /// A context that represents a result of a command request.
    /// </summary>
    public class CommandResultContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandResultContext" />.
        /// </summary>
        /// <param name="commandContext">The current command context.</param>
        public CommandResultContext(CommandContext commandContext)
        {
            CommandContext = commandContext;
        }

        /// <summary>
        /// The current command context.
        /// </summary>
        public virtual CommandContext CommandContext { get; }

        /// <summary>
        /// Gets or sets if the result was cancelled.
        /// </summary>
        public virtual bool Cancel { get; set; }
    }
}