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
    }
}