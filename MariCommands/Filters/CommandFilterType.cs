namespace MariCommands.Filters
{
    /// <summary>
    /// Defines all available filters types.
    /// </summary>
    public enum CommandFilterType
    {
        /// <summary>
        /// Invoked after a command execution.
        /// </summary>
        Result,

        /// <summary>
        /// Invoked after an exception occurred.
        /// </summary>
        Exception,
    }
}