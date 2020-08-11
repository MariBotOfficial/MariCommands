using System;
using MariCommands.TypeParsers;
using MariCommands.Utils;

namespace MariCommands
{
    /// <summary>
    /// Override the type parser for this param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class TypeParserAttribute : Attribute
    {
        /// <summary>
        /// The type of the <see cref="ITypeParser" />.
        /// </summary>
        public Type Value { get; }

        /// <summary>
        /// Defines the <see cref="ITypeParser" /> for this param.
        /// </summary>
        /// <param name="typeParserType"></param>
        /// <exception cref="ArgumentNullException">
        /// <param ref="typeParserType" /> must not be null.
        /// </exception>
        public TypeParserAttribute(Type typeParserType)
        {
            typeParserType.NotNull(nameof(typeParserType));

            Value = typeParserType;
        }
    }
}