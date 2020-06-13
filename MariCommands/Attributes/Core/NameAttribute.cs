using System;

namespace MariCommands
{
    /// <summary>
    /// Defines name of this module, command or param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class NameAttribute : Attribute
    {
        /// <summary>
        /// The name of this module, command or param.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="NameAttribute" />.
        /// </summary>
        /// <param name="name">The name of this module, command or param.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="name" /> must not be null or white space.
        /// </exception>
        public NameAttribute(string name)
        {
            name.NotNullOrWhiteSpace(nameof(name));

            Value = name;
        }
    }
}