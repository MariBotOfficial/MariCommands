using System;
using System.Runtime.ExceptionServices;

namespace MariCommands.Filters
{
    /// <summary>
    /// Represents a command exception context.
    /// </summary>
    public class CommandExceptionContext : IFilterContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandExceptionContext" />.
        /// </summary>
        /// <param name="commandContext">The current command context.</param>
        /// <param name="exceptionDispatchInfo">The exception dispatch info captured.</param>
        public CommandExceptionContext(CommandContext commandContext, ExceptionDispatchInfo exceptionDispatchInfo)
        {
            ExceptionDispatchInfo = exceptionDispatchInfo;
            CommandContext = commandContext;
        }

        /// <summary>
        /// The exception dispatch info captured.
        /// </summary>
        public virtual ExceptionDispatchInfo ExceptionDispatchInfo { get; }

        /// <summary>
        /// The exception captured.
        /// </summary>
        public virtual Exception Exception => ExceptionDispatchInfo.SourceException;

        /// <inheritdoc />
        public virtual CommandContext CommandContext { get; }

        /// <summary>
        /// Gets or sets if the exception was handled.
        /// </summary>
        public virtual bool ExceptionHandled { get; set; }
    }
}