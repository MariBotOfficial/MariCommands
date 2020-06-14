using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MariCommands
{
    /// <summary>
    /// Mark a class as group of the parent module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GroupAttribute : Attribute
    {
        /// <summary>
        /// The aliases of this group.
        /// </summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// Creates a new instance of <see cref="GroupAttribute" />.
        /// </summary>
        /// <param name="aliases">The aliases of this group.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="aliases" /> can not be null or empty.
        /// </exception>
        public GroupAttribute(params string[] aliases)
        {
            aliases.NotNullOrEmpty(nameof(aliases));

            Aliases = aliases.ToImmutableHashSet();
        }
    }
}