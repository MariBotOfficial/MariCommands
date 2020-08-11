using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents a success type parser result.
    /// </summary>
    public class TypeParserSuccessResult<T> : ITypeParserResult<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeParserSuccessResult{T}" /> with the parsed value.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        public TypeParserSuccessResult(T value)
        {
            Value = value;
        }

        /// <inheritdoc />
        public T Value { get; }

        /// <inheritdoc />
        public bool Success => true;

        /// <inheritdoc />
        public string Reason => null;

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Creates a new instance of <see cref="TypeParserSuccessResult{T}" /> with the parsed value.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A <see cref="TypeParserSuccessResult{T}" />.</returns>
        public static TypeParserSuccessResult<T> FromValue(T value)
            => new TypeParserSuccessResult<T>(value);
    }
}