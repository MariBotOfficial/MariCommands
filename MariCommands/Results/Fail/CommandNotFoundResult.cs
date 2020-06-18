using System;

namespace MariCommands
{
    /// <summary>
    /// Represents a command not found result.
    /// </summary>
    public class CommandNotFoundResult : IResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandNotFoundResult" />.
        /// </summary>
        /// <param name="input">The input used to search commands.</param>
        public CommandNotFoundResult(string input)
        {
            Input = input;
            Reason = $"Cannot found any command with raw input: {Input}.";
        }

        /// <summary>
        /// The input used to search commands.
        /// </summary>
        public string Input { get; }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Creates a <see cref="CommandNotFoundResult" /> with a raw input.
        /// </summary>
        /// <param name="input">The raw input of the searc.</param>
        /// <returns>A <see cref="CommandNotFoundResult" />.</returns>
        public static CommandNotFoundResult FromInput(string input)
            => new CommandNotFoundResult(input);
    }
}