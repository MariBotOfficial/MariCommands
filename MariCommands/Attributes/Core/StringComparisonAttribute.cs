using System;

namespace MariCommands
{
    /// <summary>
    /// Defines the <see cref="StringComparison" /> for search commands in this module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class StringComparisonAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="StringComparison" /> for search commands in this module or command.
        /// </summary>
        public StringComparison Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="StringComparisonAttribute" />.
        /// </summary>
        /// <param name="comparisonType">The <see cref="StringComparison" /> for search commands in this module or command.</param>
        public StringComparisonAttribute(StringComparison comparisonType)
        {
            Value = comparisonType;
        }
    }
}