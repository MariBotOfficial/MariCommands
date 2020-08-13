using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands
{
    /// <summary>
    /// Represents a Command service that can execute commands.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Search all modules in your project and add them to the module cache.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        IReadOnlyCollection<IModule> AddModules(Assembly assembly);

        /// <summary>
        /// Add this module to the module cache.
        /// </summary>
        /// <param name="module">Any module.</param>
        IModule AddModule(IModule module);

        /// <summary>
        /// Add this module to the module cache.
        /// </summary>
        /// <param name="builder">Any module builder.</param>
        IModule AddModule(IModuleBuilder builder)
            => AddModule(builder.Build(null));

        /// <summary>
        /// Add this module type to the module cache.
        /// </summary>
        /// <typeparam ref="T">Any module type.</typeparam>
        IModule AddModule<T>()
            where T : class
        {
            return AddModule(typeof(T));
        }


        /// <summary>
        /// Add this module type to the module cache.
        /// </summary>
        /// <param name="type">Any module type.</param>
        IModule AddModule(Type type);

        /// <summary>
        /// Remove this module from the module cache.
        /// </summary>
        /// <param name="module">The module to be removed.</param>
        void RemoveModule(IModule module);

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