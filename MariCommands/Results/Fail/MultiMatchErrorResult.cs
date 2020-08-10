using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents a multi match handling error result.
    /// </summary>
    public class MultiMatchErrorResult : IResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="MultiMatchErrorResult" />.
        /// </summary>
        /// <param name="matches">The multi matches where is not possible to handle.</param>
        public MultiMatchErrorResult(IReadOnlyCollection<ICommandMatch> matches)
        {
            Matches = matches;

            Reason = "Not possible to handle these command matches.";
        }

        /// <summary>
        /// The multi matches that cannot be handled.
        /// </summary>
        public IReadOnlyCollection<ICommandMatch> Matches { get; }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Creates a new instance of <see cref="MultiMatchErrorResult" /> with the specified matches.
        /// </summary>
        /// <param name="matches">The multi matches where is not possible to handle.</param>
        /// <returns>A <see cref="MultiMatchErrorResult" />.</returns>
        public static MultiMatchErrorResult FromMatches(IReadOnlyCollection<ICommandMatch> matches)
            => new MultiMatchErrorResult(matches);
    }
}