using System;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class TypeParserResult : ITypeParserResult
    {
        /// <inheritdoc />
        public bool Success
            => string.IsNullOrWhiteSpace(Reason);

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public bool HasValue { get; }

        /// <inheritdoc />
        public object Value { get; }

        /// <inheritdoc />
        public Exception Exception { get; }

        /// <summary>
        /// Creates a new instance of <see cref="TypeParserResult" />.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        public TypeParserResult(object value)
        {
            HasValue = value.HasContent();
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TypeParserResult" />.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="exception" /> must not be null.
        /// </exception>
        public TypeParserResult(Exception exception)
        {
            exception.NotNull(nameof(exception));

            Exception = exception;
            Reason = exception.Message;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TypeParserResult" />.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="reason" /> must not be null or white space.
        /// </exception>
        public TypeParserResult(string reason)
        {
            reason.NotNullOrWhiteSpace(nameof(reason));

            Reason = reason;
        }

        /// <summary>
        /// Build a success type parser result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A success type parser result.</returns>
        static TypeParserResult FromSuccess(object value)
            => new TypeParserSuccessResult(value);

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed type parser result.</returns>
        static TypeParserResult FromError(string reason)
            => new TypeParserFailResult(reason);

        /// <summary>
        /// Build a faile type parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed type parser result.</returns>
        static TypeParserResult FromException(Exception exception)
            => new TypeParserFailResult(exception);
    }
}