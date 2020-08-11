using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents a type reader result.
    /// </summary>
    public interface ITypeParserResult : IResult
    {
        /// <summary>
        /// The value of this result.
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// Represents a type reader result.
    /// </summary>
    public interface ITypeParserResult<T> : ITypeParserResult
    {
        object ITypeParserResult.Value => Value;

        /// <summary>
        /// The value of this result.
        /// </summary>
        new T Value { get; }
    }
}