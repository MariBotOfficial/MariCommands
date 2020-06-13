using System;

namespace MariCommands
{
    /// <summary>
    /// Defines a description for this module, command or param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// The description of this module, command or param.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="DescriptionAttribute" />.
        /// </summary>
        /// <param name="description">The description of this module, command or param.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="description" /> must not be null or white space.
        /// </exception>
        public DescriptionAttribute(string description)
        {
            description.NotNullOrWhiteSpace(nameof(description));

            Value = description;
        }
    }
}