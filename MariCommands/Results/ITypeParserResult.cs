using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents a type reader result.
    /// </summary>
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
        /// Converts this result to an <see cref="ITypeParserResult{T}"/> of an object.
        /// </summary>
        /// <returns>An <see cref="ITypeParserResult{T}"/>.</returns>
        ITypeParserResult<object> ConvertToObject();
    }
}