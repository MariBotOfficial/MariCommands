using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when a type parser fails to parse an argument.
    /// </summary>
    public class TypeParserFailResult<T> : IArgumentParserResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="TypeParserFailResult{T}" /> with the specified
        /// <see cref="ITypeParserResult{T}" />.
        /// </summary>
        /// <param name="typeParserResult">The result returned from the type parser.</param>
        public TypeParserFailResult(ITypeParserResult<T> typeParserResult)
        {
            TypeParserResult = typeParserResult;
        }

        /// <summary>
        /// The result returned from the type parser.
        /// </summary>
        public ITypeParserResult<T> TypeParserResult { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<IParameter, object> Args => null;

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => TypeParserResult.Reason;

        /// <inheritdoc />
        public Exception Exception => TypeParserResult.Exception;

        /// <summary>
        /// Creates a new instance of<see cref= "TypeParserFailResult{T}" /> with the specified
        /// <see cref="ITypeParserResult{T}" />.
        /// </summary>
        /// <param name="typeParserResult">The result returned from the type parser.</param>
        /// <returns>A <see cref="TypeParserFailResult{T}" />.</returns>
        public static TypeParserFailResult<T> FromTypeParserResult(ITypeParserResult<T> typeParserResult)
            => new TypeParserFailResult<T>(typeParserResult);
    }
}