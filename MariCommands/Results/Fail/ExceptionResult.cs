using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when a execution fails.
    /// </summary>
    public class ExceptionResult : IResult
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionResult" /> with the specified exception.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        public ExceptionResult(Exception exception)
        {
            Exception = exception;
        }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => $"An unexpected error occurred {Exception.Message}";

        /// <inheritdoc />
        public Exception Exception { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionResult" /> with the specified exception.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <returns>A <see cref="ExceptionResult" />.</returns>
        public static ExceptionResult FromException(Exception exception)
            => new ExceptionResult(exception);
    }
}