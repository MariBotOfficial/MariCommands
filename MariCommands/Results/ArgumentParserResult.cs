using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MariCommands
{
    /// <inheritdoc />
    public class ArgumentParserResult : IArgumentParserResult
    {
        /// <inheritdoc />
        public IReadOnlyDictionary<IParameter, object> Args { get; }

        /// <inheritdoc />
        public bool Success => string.IsNullOrWhiteSpace(Reason);

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParserResult" />.
        /// </summary>
        /// <param name="args">All parsed args.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="args" /> can not be null or empty.
        /// </exception>
        public ArgumentParserResult(IDictionary<IParameter, object> args)
        {
            args.NotNullOrEmpty(nameof(args));

            Args = args.ToImmutableDictionary();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParserResult" />.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="reason" /> must not be null or white space.
        /// </exception>
        public ArgumentParserResult(string reason)
        {
            reason.NotNullOrWhiteSpace(nameof(reason));

            Reason = reason;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParserResult" />.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <exception cref="ArgumentNullException"> 
        /// <paramref name="exception" /> must not be null.
        /// </exception>
        public ArgumentParserResult(Exception exception)
        {
            exception.NotNull(nameof(exception));

            Exception = exception;
            Reason = exception.Message;
        }

        /// <summary>
        /// Build a success argument parser result.
        /// </summary>
        /// <param name="args">All parsed args.</param>
        /// <returns>A success argument parser result.</returns>
        public static ArgumentParserResult FromSuccess(IDictionary<IParameter, object> args)
            => new ArgumentParserSuccessResult(args);

        /// <summary>
        /// Build a failed argument parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed argument parser result.</returns>
        public static ArgumentParserResult FromError(string reason)
            => new ArgumentParserFailResult(reason);

        /// <summary>
        /// Build a failed argument parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed argument parser result.</returns>
        public static ArgumentParserResult FromException(Exception exception)
            => new ArgumentParserFailResult(exception);
    }
}