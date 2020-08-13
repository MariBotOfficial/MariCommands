using System;
using System.Collections.Generic;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when the command parameters can sucessfully parsed.
    /// </summary>
    public class ArgumentParseSuccessResult : IArgumentParserResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParseSuccessResult" /> with the specified args.
        /// </summary>
        /// <param name="args">The parsed args.</param>
        public ArgumentParseSuccessResult(IReadOnlyDictionary<IParameter, object> args)
        {
            Args = args;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<IParameter, object> Args { get; }

        /// <inheritdoc />
        public bool Success => true;

        /// <inheritdoc />
        public string Reason => null;

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParseSuccessResult" /> with the specified args.
        /// </summary>
        /// <param name="args">The parsed args.</param>
        /// <returns>An <see cref="ArgumentParseSuccessResult" />.</returns>
        public static ArgumentParseSuccessResult FromArgs(IReadOnlyDictionary<IParameter, object> args)
            => new ArgumentParseSuccessResult(args);
    }
}