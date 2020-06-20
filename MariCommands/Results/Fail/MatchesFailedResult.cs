using System;
using System.Collections.Generic;

namespace MariCommands
{
    /// <summary>
    /// Represents one or more 
    /// </summary>
    public class MatchesFailedResult : IResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="MatchesFailedResult" />.
        /// </summary>
        /// <param name="failedCommands">All failed command and yours results.</param>
        public MatchesFailedResult(IReadOnlyDictionary<ICommand, IResult> failedCommands)
        {
            FailedCommands = failedCommands;
        }

        /// <summary>
        /// All failed command and yours results.
        /// </summary>
        public IReadOnlyDictionary<ICommand, IResult> FailedCommands { get; }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => $"Failed to find a valid command match.";

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Returns a <see cref="MatchesFailedResult" /> with the specified failed commands.
        /// </summary>
        /// <param name="failedCommands">All failed command and yours results.</param>
        /// <returns>A <see cref="MatchesFailedResult" />.</returns>
        public static MatchesFailedResult FromFaileds(IReadOnlyDictionary<ICommand, IResult> failedCommands)
            => new MatchesFailedResult(failedCommands);
    }
}