using System.Collections.Generic;

namespace MariCommands.Features
{
    /// <summary>
    /// A feature to get all matched commands for this context.
    /// </summary>
    public interface ICommandMatchesFeature
    {
        /// <summary>
        /// Gets or sets sall matched commands for this context.
        /// </summary>
        IReadOnlyCollection<ICommandMatch> CommandMatches { get; set; }
    }
}