namespace MariCommands
{
    /// <summary>
    /// An interface taht can access the current <see cref="CommandContext" /> of
    /// the current scoped execution context.
    /// </summary>
    public interface ICommandContextAccessor
    {
        /// <summary>
        /// Gets or sets the current <see cref="CommandContext" /> of the current scoped 
        /// execution context.
        /// </summary>
        CommandContext Context { get; set; }
    }
}