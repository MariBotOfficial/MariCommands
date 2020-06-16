using System.Collections.Generic;

namespace MariCommands
{
    /// <summary>
    /// Represents an execution context of a command.
    /// </summary>
    public class CommandExecutionContext
    {
        /// <summary>
        /// Represents a command context.
        /// </summary>
        public CommandContext CommandContext { get; }

        /// <summary>
        /// The result of this execution context.
        /// </summary>
        public IResult Result { get; set; }

        /// <summary>
        /// A key/value collection to share data within the execution.
        /// </summary>
        public IDictionary<object, object> Items { get; set; }
    }
}