using System;

namespace MariCommands
{
    /// <summary>
    /// General config for <see cref="ICommandService" />
    /// </summary>
    public interface ICommandServiceOptions
    {
        /// <summary>
        /// Gets or sets the default lifetime for modules.
        /// </summary>
        ModuleLifetime ModuleLifetime { get; set; }

        /// <summary>
        /// Gets or sets the default runmode for commands.
        /// </summary>
        RunMode RunMode { get; set; }

        /// <summary>
        /// Gets or sets the default comparison for search commands.
        /// </summary>
        StringComparison? Comparison { get; set; }

        /// <summary>
        /// Gets or sets if extra args in a command will be ignored.
        /// </summary>
        bool IgnoreExtraArgs { get; set; }

        /// <summary>
        /// Gets or sets how multi matches will be handled.
        /// </summary>
        MultiMatchHandling MatchHandling { get; set; }

        /// <summary>
        /// Gets or sets the default command separator to be used for commands and groups.
        /// </summary>
        string Separator { get; set; }
    }
}