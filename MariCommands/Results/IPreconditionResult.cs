using System;

namespace MariCommands
{
    /// <summary>
    /// Represents a result for a <see cref="PreconditionAttribute" />.
    /// </summary>
    public interface IPreconditionResult : IResult
    {
        /// <summary>
        /// Build a success precondition result.
        /// </summary>
        /// <returns>A success precondition result.</returns>
        static IPreconditionResult FromSuccess()
            => new PreconditionSuccessResult();

        /// <summary>
        /// Build a failed precondition result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed precondition result.</returns>
        static IPreconditionResult FromError(string reason)
            => new PreconditionFailResult(reason);

        /// <summary>
        /// Build a failed precondition result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the precondition check.</param>
        /// <returns>A failed precondition result.</returns>
        static IPreconditionResult FromException(Exception exception)
            => new PreconditionFailResult(exception);
    }
}