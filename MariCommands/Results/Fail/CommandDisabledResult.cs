using System;

namespace MariCommands
{
    /// <summary>
    /// Represents a command cannot execute because he's disabled.
    /// </summary>
    public class CommandDisabledResult : IResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandDisabledResult" />.
        /// </summary>
        /// <param name="command">The command that is disabled.</param>
        public CommandDisabledResult(ICommand command)
        {
            Command = command;
        }

        /// <summary>
        /// The command that is disabled.
        /// </summary>
        public ICommand Command { get; }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => $"The command {Command.Name} cannot be executed because he's disabled.";

        /// <inheritdoc />
        public Exception Exception => null;
    }
}