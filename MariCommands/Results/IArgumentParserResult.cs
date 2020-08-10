using System;
using System.Collections.Generic;
using MariCommands.Parsers;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents an result for an <see cref="IArgumentParser" />.
    /// </summary>
    public interface IArgumentParserResult : IResult
    {
        /// <summary>
        /// All parsed args.
        /// </summary>
        IReadOnlyDictionary<IParameter, object> Args { get; }
    }
}