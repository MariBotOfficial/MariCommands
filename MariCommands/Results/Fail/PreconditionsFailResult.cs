using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when one or more preconditions fails.
    /// </summary>
    public class PreconditionsFailResult : IResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="PreconditionsFailResult" /> with the 
        /// specified failed preconditions results.
        /// </summary>
        /// <param name="command">The command that these preconditions failed.</param>
        /// <param name="failedPreconditions">All failed preconditions results.</param>
        public PreconditionsFailResult(ICommand command, IReadOnlyCollection<(PreconditionAttribute Precondition, IPreconditionResult Result)> failedPreconditions)
        {
            Command = command;
            FailedPreconditions = failedPreconditions;
            Reason = GetReason();
        }

        private string GetReason()
        {
            var oneManyText = FailedPreconditions.Count > 1
                ? "Many preconditions"
                : "One precondition";

            return $"{oneManyText} failed for the command {Command.Name}";
        }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// The command that failed.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// All failed preconditions.
        /// </summary>
        public IReadOnlyCollection<(PreconditionAttribute Precondition, IPreconditionResult Result)> FailedPreconditions { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionsFailResult" /> with the 
        /// specified failed preconditions results.
        /// </summary>
        /// <param name="command">The command that these preconditions failed.</param>
        /// <param name="fails">All failed preconditions results.</param>
        /// <returns>A <see cref="PreconditionsFailResult" />.</returns>
        public static PreconditionsFailResult FromFaileds(ICommand command, IReadOnlyCollection<(PreconditionAttribute, IPreconditionResult)> fails)
            => new PreconditionsFailResult(command, fails);
    }
}