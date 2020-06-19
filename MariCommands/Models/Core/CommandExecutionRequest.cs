namespace MariCommands
{
    /// <summary>
    /// Represents a request for execute a context.
    /// </summary>
    public class CommandExecutionRequest
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandExecutionContext" />.
        /// </summary>
        /// <param name="instance">The instance of the module.</param>
        /// <param name="context">The current command execution context.</param>
        /// <param name="needsDispose">If the module needs to be disposed.</param>
        public CommandExecutionRequest(object instance, CommandExecutionContext context, bool needsDispose)
        {
            Instance = instance;
            Context = context;
            NeedsDispose = needsDispose;
        }

        /// <summary>
        /// The instance of the module.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// The current command execution context.
        /// </summary>
        public CommandExecutionContext Context { get; }

        /// <summary>
        /// If the module needs to be disposed.
        /// </summary>
        public bool NeedsDispose { get; }
    }
}