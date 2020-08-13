using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands
{
    /// <summary>
    /// A service that can execute commands.
    /// </summary>
    public interface IContextExecutor
    {
        /// <summary>
        /// Execute a command with the specified text input and returns the result.
        /// </summary>
        /// <param name="input">The specified text input.</param>
        /// <param name="commandContext">The command context for use in the execution.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="IResult" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="input" /> must not be null or white space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="commandContext" /> must not be null.
        /// </exception>
        Task<IResult> ExecuteAsync(string input, CommandContext commandContext);

        /// <summary>
        /// Execute this command with the specified raw args and returns the result.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="args">The args of the command.</param>
        /// <param name="commandContext">The command context for use in the execution.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="IResult" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="command" /> must not be null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="args" /> must not be null or white space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="commandContext" /> must not be null.
        /// </exception>
        Task<IResult> ExecuteAsync(ICommand command, string args, CommandContext commandContext);

        /// <summary>
        /// Execute this command with the specified args and returns the results.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        /// <param name="args">The parsed args of the command.</param>
        /// <param name="commandContext">The command context for use in the execution.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="IResult" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="command" /> must not be null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="args" /> must not be null or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     <param ref="commandContext" /> must not be null.
        /// </exception>
        Task<IResult> ExecuteAsync(ICommand command, IEnumerable<object> args, CommandContext commandContext);

        /// <summary>
        /// Initialize the current command service with the specified command middleware pipeline.
        /// </summary>
        /// <param name="commandDelegate">The command middleware for proccess command requests.</param>
        void Initialize(CommandDelegate commandDelegate);
    }
}