using System;
using System.Threading.Tasks;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <summary>
    /// When implemented will mark the object as a <see cref="IModule" />.
    /// </summary>
    /// <typeparam name="T">The <see cref="CommandContext" /> this module will use.</typeparam>
    public class ModuleBase<T> : IModuleBase
        where T : CommandContext
    {
        /// <summary>
        /// The command context in this module.
        /// </summary>
        public T Context { get; private set; }

        /// <summary>
        /// An asynchronous operation that will be executed before the command execution.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        public virtual Task OnCommandExecutingAsync()
            => Task.CompletedTask;

        /// <summary>
        /// An asynchronous operation that will be executed after the command execution.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation.</returns>
        public virtual async Task OnCommandExecutedAsync()
        {
            var options = Context?.CommandServices?.GetService<MariCommandsOptions>();

            if (options.HasNoContent() || !options.AutoDisposeContext)
                return;

            await Context.DisposeAsync();
        }

        void IModuleBase.SetContext(CommandContext context)
        {
            if (!context.OfType<T>())
                throw new InvalidCastException($"Cannot cast the current context of type {context.GetType()} to {typeof(T)}.");

            Context = context as T;
        }

        /// <summary>
        /// Creates a <see cref="SuccessResult" /> object.
        /// </summary>
        /// <returns>The created <see cref="SuccessResult" />. </returns>
        protected virtual SuccessResult Success()
            => new SuccessResult();

        /// <summary>
        /// Creates a <see cref="SuccessObjectResult" /> object.
        /// </summary>
        /// <param name="value">The content value to be associated with the result.</param>
        /// <returns>The created <see cref="SuccessObjectResult" />. </returns>
        protected virtual SuccessObjectResult Success(object value)
            => new SuccessObjectResult(value);

        /// <summary>
        /// Creates a <see cref="SuccessResult" /> object.
        /// </summary>
        /// <param name="exception">The exception to be associated with the result.</param>
        /// <returns>The created <see cref="SuccessResult" />. </returns>
        protected virtual ExceptionResult Exception(Exception exception)
            => new ExceptionResult(exception);
    }
}