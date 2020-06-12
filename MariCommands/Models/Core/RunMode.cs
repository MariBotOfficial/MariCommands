namespace MariCommands
{
    /// <summary>
    /// Defines the run type execution of a command.
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// The commands will be executed sequentially.
        /// </summary>
        Sequential = 0,

        /// <summary>
        /// The commands will be executed concurrently.
        /// </summary>
        Concurrent = 1,
    }
}