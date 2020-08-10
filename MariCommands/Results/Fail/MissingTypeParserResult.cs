using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when the type doesn't has the necessary type parser registered.
    /// </summary>
    public class MissingTypeParserResult : IArgumentParserResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="MissingTypeParserResult" /> with the specified param.
        /// </summary>
        /// <param name="parameter">The parameter that cannot be parsed.</param>
        public MissingTypeParserResult(IParameter parameter)
        {
            Parameter = parameter;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<IParameter, object> Args => null;

        /// <inheritdoc />
        public bool Success => false;

        /// <summary>
        /// The parameter that cannot be parsed.
        /// </summary>
        public IParameter Parameter { get; set; }

        /// <inheritdoc />
        public string Reason => $"Cannot found a type parser for argument: {Parameter.Name}";

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Creates a new instance of <see cref="MissingTypeParserResult" /> with the specified param.
        /// </summary>
        /// <param name="param">The parameter that cannot be parsed.</param>
        /// <returns>A <see cref="MissingTypeParserResult" />.</returns>
        public static MissingTypeParserResult FromParam(IParameter param)
            => new MissingTypeParserResult(param);
    }
}