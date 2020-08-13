using System.Collections.Generic;

namespace MariCommands.Features
{
    internal sealed class CommandMatchesFeature : ICommandMatchesFeature
    {
        public IReadOnlyCollection<ICommandMatch> CommandMatches { get; set; }
    }
}