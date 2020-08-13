using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MariCommands.Utils;

namespace MariCommands
{
    /// <summary>
    /// Mark a method to be a Command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// All aliases for this command.
        /// </summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CommandAttribute" />.
        /// </summary>
        /// <param name="aliases">All aliases for this command.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="aliases" /> can not be null or empty.
        /// </exception>
        public CommandAttribute(params string[] aliases)
        {
            aliases.NotNullOrEmpty(nameof(aliases));

            Aliases = aliases.ToImmutableArray();
        }
    }
}