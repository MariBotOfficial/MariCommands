namespace MariCommands
{
    /// <summary>
    /// Defines how multi matches will be handled.
    /// </summary>
    public enum MultiMatchHandling
    {
        /// <summary>
        /// An error will be returned.
        /// </summary>
        Error = 0,

        /// <summary>
        /// The best command overload will be selected.
        /// </summary>
        Best = 1,
    }
}