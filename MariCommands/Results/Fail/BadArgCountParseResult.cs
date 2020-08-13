using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents whens the input doesn't has necessary the params.
    /// </summary>
    public class BadArgCountParseResult : IArgumentParserResult
    {
        /// <summary>
        /// Create a new instance of <see cref="BadArgCountParseResult" />.
        /// </summary>
        /// <param name="command">The command with the bad arg count.</param>
        public BadArgCountParseResult(ICommand command)
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
        public string Reason => $"The command {Command.Name} cannot be parsed because has missing args.";

        /// <inheritdoc />
        public Exception Exception => null;

        /// <inheritdoc />
        public IReadOnlyDictionary<IParameter, object> Args => null;

        /// <summary>
        /// Creates a <see cref="BadArgCountParseResult" /> with the specified command.
        /// </summary>
        /// <param name="command">The command with the bad arg count.</param>
        /// <returns>A <see cref="BadArgCountParseResult" />.</returns>
        public static BadArgCountParseResult FromCommand(ICommand command)
            => new BadArgCountParseResult(command);
    }
}