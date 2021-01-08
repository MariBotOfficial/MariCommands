using MariCommands.Results;

namespace MariCommands.Filters
{
    /// <summary>
    /// A context that represents a result of a command request.
    /// </summary>
    public class CommandResultContext : IFilterContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandResultContext" />.
        /// </summary>
        /// <param name="commandContext">The current command context.</param>
        /// <param name="result">The result of the command.</param>
        public CommandResultContext(CommandContext commandContext, IResult result)
        {
            CommandContext = commandContext;
            Result = result;
        }

        /// <inheritdoc />
        public virtual CommandContext CommandContext { get; }

        /// <inheritdoc />
        public virtual IResult Result { get; }

        /// <summary>
        /// Gets or sets if the result was cancelled.
        /// </summary>
        public virtual bool Cancel { get; set; }
    }
}