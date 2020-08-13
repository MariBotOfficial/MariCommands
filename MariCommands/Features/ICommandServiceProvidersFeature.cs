using System;

namespace MariCommands.Features
{
    /// <summary>
    /// A feature to gets or sets the command services of this command execution.
    /// </summary>
    public interface ICommandServiceProvidersFeature
    {
        /// <summary>
        /// Gets or sets the command services of this command execution.
        /// </summary>
        IServiceProvider CommandServices { get; set; }
    }
}