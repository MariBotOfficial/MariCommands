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

        // TODO: change specified success and fail results.

        /// <summary>
        /// Build a success type parser result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A success type parser result.</returns>
        static ITypeParserResult<T> FromSuccess(T value)
            => new TypeParserResult<T>(value);

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed type parser result.</returns>
        static ITypeParserResult<T> FromError(string reason)
            => new TypeParserResult<T>(reason);

        /// <summary>
        /// Build a faile type parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed type parser result.</returns>
        static ITypeParserResult<T> FromException(Exception exception)
            => new TypeParserResult<T>(exception);
    }
}