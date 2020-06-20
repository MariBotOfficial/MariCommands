using System;

namespace MariCommands
{
    /// <summary>
    /// Represents whens the input doesn't has necessary the params.
    /// </summary>
    public class BadArgCountResult : IResult
    {
        /// <summary>
        /// Create a new instance of <see cref="BadArgCountResult" />.
        /// </summary>
        /// <param name="command">The command with the bad arg count.</param>
        public BadArgCountResult(ICommand command)
        {
            Command = command;
        }

        /// <summary>
        /// The command with the bad arg count.
        /// </summary>
        public ICommand Command { get; }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => $"The command {Command.Name} cannot be executed because has missing args.";

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Creates a <see cref="BadArgCountResult" /> with the specified command.
        /// </summary>
        /// <param name="command">The command with the bad arg count.</param>
        /// <returns>A <see cref="BadArgCountResult" />.</returns>
        public static BadArgCountResult FromCommand(ICommand command)
            => new BadArgCountResult(command);
    }
}