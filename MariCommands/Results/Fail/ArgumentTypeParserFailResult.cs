using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when a type parser fails to parse an argument.
    /// </summary>
    public class ArgumentTypeParserFailResult : IArgumentParserResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArgumentTypeParserFailResult" /> with the specified
        /// <see cref="ITypeParserResult" />.
        /// </summary>
        /// <param name="typeParserResult">The result returned from the type parser.</param>
        public ArgumentTypeParserFailResult(ITypeParserResult typeParserResult)
        {
            TypeParserResult = typeParserResult;
        }

        /// <summary>
        /// The result returned from the type parser.
        /// </summary>
        public ITypeParserResult TypeParserResult { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<IParameter, object> Args => null;

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => TypeParserResult.Reason;

        /// <inheritdoc />
        public Exception Exception => TypeParserResult.Exception;

        /// <summary>
        /// Creates a new instance of<see cref= "ArgumentTypeParserFailResult" /> with the specified
        /// <see cref="ITypeParserResult" />.
        /// </summary>
        /// <param name="typeParserResult">The result returned from the type parser.</param>
        /// <returns>A <see cref="ArgumentTypeParserFailResult" />.</returns>
        public static ArgumentTypeParserFailResult FromTypeParserResult(ITypeParserResult typeParserResult)
            => new ArgumentTypeParserFailResult(typeParserResult);
    }
}