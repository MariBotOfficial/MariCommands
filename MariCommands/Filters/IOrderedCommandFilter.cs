namespace MariCommands.Filters
{
    /// <summary>
    /// Represents an ordered filter.
    /// </summary>
    public interface IOrderedCommandFilter : ICommandFilter
    {
        /// <summary>
        /// Gets the order of this filter.
        /// </summary>
        int Order { get; }
    }
}