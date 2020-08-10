using System;

namespace MariCommands
{
    /// <summary>
    /// Represents a type reader result.
    /// </summary>/*  */
    public interface ITypeParserResult : IResult
    {
        /// <summary>
        /// If this result has any value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// The value of this result.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Build a success type parser result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A success type parser result.</returns>
        static ITypeParserResult FromSuccess(object value)
            => new TypeParserSuccessResult(value);

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed type parser result.</returns>
        static ITypeParserResult FromError(string reason)
            => new TypeParserFailResult(reason);

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed type parser result.</returns>
        static ITypeParserResult FromException(Exception exception)
            => new TypeParserFailResult(exception);
    }
}