using System;

namespace MariCommands
{
    /// <summary>
    /// Represents a type reader result.
    /// </summary>
    /// <typeparam name="T">The type handled by the type parser.</typeparam>
    public interface ITypeParserResult<T> : IResult
    {
        /// <summary>
        /// If this result has any value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// The value of this result.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Build a success type parser result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A success type parser result.</returns>
        static ITypeParserResult<T> FromSuccess(T value)
            => new TypeParserSuccessResult<T>(value);

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed type parser result.</returns>
        static ITypeParserResult<T> FromError(string reason)
            => new TypeParserFailResult<T>(reason);

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed type parser result.</returns>
        static ITypeParserResult<T> FromException(Exception exception)
            => new TypeParserFailResult<T>(exception);
    }
}