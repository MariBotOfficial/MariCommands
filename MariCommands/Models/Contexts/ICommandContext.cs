using System.Collections.Generic;

namespace MariCommands.Models.Contexts
{
    /// <summary>
    /// Represents a command context execution.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// The currently command of this context.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// The alias used for this command.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The raw arguments of this context.
        /// </summary>
        string RawArgs { get; }

        /// <summary>
        /// The parsed arguments.
        /// </summary>
        IReadOnlyCollection<object> Args { get; }
    }
}