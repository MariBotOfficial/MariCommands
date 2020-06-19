namespace MariCommands
{
    /// <summary>
    /// A service that can instantiate a module.
    /// </summary>
    public interface IModuleProvider
    {
        /// <summary>
        /// Instantiate the module.
        /// </summary>
        /// <param name="context">The current command context execution.</param>
        /// <returns>The module instantiated.</returns>
        CommandExecutionRequest Instantiate(CommandExecutionContext context);
    }
}