using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents a nullable type parser result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NullableTypeParserResult<T> : ITypeParserResult<T?>
        where T : struct
    {
        /// <summary>
        /// Creates a new instance of <see cref="NullableTypeParserResult{T}" />.
        /// </summary>
        public NullableTypeParserResult()
        {
            Value = null;
            Success = true;
            Reason = null;
            Exception = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="NullableTypeParserResult{T}" /> with the specified
        /// result.
        /// </summary>
        /// <param name="result">The original type parser result.</param>
        public NullableTypeParserResult(ITypeParserResult<T> result)
        {
            RealResult = result;
            Value = result.Value;
            Success = result.Success;
            Reason = result.Reason;
            Exception = result.Exception;
        }

        /// <summary>
        /// The real result of the type parser.
        /// </summary>
        /// <remarks>May be null.</remarks>
        public ITypeParserResult<T> RealResult { get; }

        /// <summary>
        /// If this result has any value.
        /// </summary>
        public bool HasValue => Value.HasValue;

        /// <inheritdoc />
        public T? Value { get; }

        /// <inheritdoc />
        public bool Success { get; }

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception { get; }

        /// <summary>
        /// Creates a new instance of <see cref="NullableTypeParserResult{T}" /> with the specified
        /// result.
        /// </summary>
        /// <param name="result">The original type parser result.</param>
        /// <returns>A <see cref="NullableTypeParserResult{T}" />.</returns>
        public static NullableTypeParserResult<T> FromResult(ITypeParserResult<T> result)
            => new NullableTypeParserResult<T>(result);
    }
}