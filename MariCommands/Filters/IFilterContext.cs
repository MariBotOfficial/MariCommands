namespace MariCommands.Filters
{
    /// <summary>
    /// Represents a filter context.
    /// </summary>
    public interface IFilterContext
    {
        /// <summary>
        /// The current command context.
        /// </summary>
        CommandContext CommandContext { get; }
    }
}