namespace MariCommands
{
    /// <summary>
    /// Defines the lifetime of the modules.
    /// </summary>
    public enum ModuleLifetime
    {
        /// <summary>
        /// The modules will be transient.
        /// </summary>
        Transient = 0,

        /// <summary>
        /// The modules will be singleton.
        /// </summary>
        Singleton = 1,
    }
}