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
        /// <param name="context">The current command context.</param>
        /// <returns>The module instantiated.</returns>
        object Instantiate(CommandContext context);
    }
}