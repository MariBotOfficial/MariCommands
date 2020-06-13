using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <summary>
    /// Mark a method to be a Command.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
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
        public CommandAttribute(params string[] aliases)
        {
            aliases.NotNullOrEmpty(nameof(aliases));

            Aliases = ImmutableHashSet.Create(aliases);
        }
    }
}