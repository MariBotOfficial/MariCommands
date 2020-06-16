using System.Collections.Generic;
using System.Collections.Immutable;

namespace MariCommands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// The currently command of this context.
        /// </summary>
        public ICommand Command { get; private set; }

        /// <summary>
        /// The alias used for this command.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The raw arguments of this context.
        /// </summary>
        public string RawArgs { get; private set; }

        /// <summary>
        /// The parsed arguments.
        /// </summary>
        public IReadOnlyCollection<object> Args { get; private set; }

        internal void SetCommandMatch(ICommandMatch match)
        {
            Command = match.Command;
            Alias = match.Alias;
            RawArgs = match.RawArgs;
        }

        internal void SetParsedArgs(IEnumerable<object> args)
        {
            Args = args.ToImmutableArray();
        }
    }
}