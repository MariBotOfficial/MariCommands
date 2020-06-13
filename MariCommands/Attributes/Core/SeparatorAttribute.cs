using System;

namespace MariCommands
{
    /// <summary>
    /// Defines the separator of the args for this module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SeparatorAttribute : Attribute
    {
        /// <summary>
        /// The separator of the args for this module or command.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance off <see cref="SeparatorAttribute" />.
        /// </summary>
        /// <param name="separator">The separator of the args for this module or command.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="separator" /> must not be null or white space.
        /// </exception>
        public SeparatorAttribute(string separator)
        {
            separator.NotNullOrWhiteSpace(nameof(separator));

            Value = separator;
        }
    }
}