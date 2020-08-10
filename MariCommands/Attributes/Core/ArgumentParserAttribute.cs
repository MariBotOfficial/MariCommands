using System;
using MariCommands.Parsers;

namespace MariCommands
{
    /// <summary>
    /// Defines the type of <see cref="IArgumentParser" /> for this module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ArgumentParserAttribute : Attribute
    {
        /// <summary>
        /// The type of <see cref="IArgumentParser" /> for this module or command.
        /// </summary>
        public Type Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParserAttribute" />.
        /// </summary>
        /// <param name="type">The type of <see cref="IArgumentParser" /> for this module or command.</param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="type" /> must not be null.
        /// </exception>
        public ArgumentParserAttribute(Type type)
        {
            type.NotNull(nameof(type));

            Value = type;
        }
    }
}