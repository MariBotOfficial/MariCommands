using System;
using System.Collections.Immutable;

namespace MariCommands
{
    /// <summary>
    /// Mark aliases for this command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// All aliases for this command.
        /// </summary>
        public IImmutableList<string> Aliases { get; }

        /// <summary>
        /// Creates a new instance off <see cref="AliasAttribute" />.
        /// </summary>
        /// <param name="aliases">All aliases for this command.</param>
        public AliasAttribute(params string[] aliases)
        {
            Aliases = ImmutableList.Create(aliases);
        }
    }
}