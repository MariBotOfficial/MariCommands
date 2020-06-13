using System;
using System.Collections.Generic;

namespace MariCommands
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

        /// <summary>
        /// Build a success argument parser result.
        /// </summary>
        /// <param name="args">All parsed args.</param>
        /// <returns>A success argument parser result.</returns>
        static IArgumentParserResult FromSuccess(IDictionary<IParameter, object> args)
            => new ArgumentParserSuccessResult(args);

        /// <summary>
        /// Build a failed argument parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed argument parser result.</returns>
        static IArgumentParserResult FromError(string reason)
            => new ArgumentParserFailResult(reason);

        /// <summary>
        /// Build a failed argument parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed argument parser result.</returns>
        static IArgumentParserResult FromException(Exception exception)
            => new ArgumentParserFailResult(exception);
    }
}