using System;
using System.Collections.Generic;

namespace MariCommands.Builder
{
    /// <summary>
    /// Represents a builder that will build the command pipeline request.
    /// </summary>
    public interface ICommandApplicationBuilder
    {
        /// <summary>
        /// Properties to be used across middlewares.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// The general service container for this application.
        /// </summary>
        IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// Add a middleware to the command execution pipeline.
        /// </summary>
        /// <param name="component">The middleware function to execute the current command request.</param>
        /// <returns>The current command application builder.</returns>
        ICommandApplicationBuilder Use(Func<CommandDelegate, CommandDelegate> component);

        /// <summary>
        /// Build the command execution pipeline.
        /// </summary>
        /// <returns></returns>
        CommandDelegate Build();
    }
}