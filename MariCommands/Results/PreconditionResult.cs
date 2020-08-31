using System;
using MariCommands.Utils;

namespace MariCommands.Results
{
    /// <inheritdoc />
    public class PreconditionResult : IPreconditionResult
    {
        /// <inheritdoc />
        public bool Success => string.IsNullOrWhiteSpace(Reason);

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionResult" />.
        /// </summary>
        public PreconditionResult()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionResult" />.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="reason" /> must not be null or white space.
        /// </exception>       
        public PreconditionResult(string reason)
        {
            reason.NotNullOrWhiteSpace(nameof(reason));

            Reason = reason;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionResult" />.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="exception" /> must not be null.
        /// </exception>
        public PreconditionResult(Exception exception)
        {
            exception.NotNull(nameof(exception));

            Exception = exception;
            Reason = exception.Message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionResult" />.
        /// </summary>
        /// <returns>
        /// A <see cref="PreconditionResult" />
        /// </returns>
        public static PreconditionResult FromSuccess()
            => new PreconditionResult();

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionResult" />.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="reason" /> must not be null or white space.
        /// </exception>
        /// <returns>
        /// A <see cref="PreconditionResult" />
        /// </returns>
        public static PreconditionResult FromErrorReason(string reason)
            => new PreconditionResult(reason);

        /// <summary>
        /// Creates a new instance of <see cref="PreconditionResult" />.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="exception" /> must not be null.
        /// </exception>
        /// <returns>
        /// A <see cref="PreconditionResult" />
        /// </returns>
        public static PreconditionResult FromException(Exception exception)
            => new PreconditionResult(exception);
    }
}