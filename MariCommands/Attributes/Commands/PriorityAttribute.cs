using System;

namespace MariCommands
{
    /// <summary>
    /// Defines the priority of this command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PriorityAttribute : Attribute
    {
        /// <summary>
        /// The priority of this command.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="PriorityAttribute" />.
        /// </summary>
        /// <param name="value">The priority of this command.</param>
        public PriorityAttribute(int value)
            => Value = value;
    }
}