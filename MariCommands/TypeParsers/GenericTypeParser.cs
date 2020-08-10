using System;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.TypeParsers
{
    /// <summary>
    /// Provides an base implementatio for generic typeparsers.
    /// </summary>
    public abstract class GenericTypeParser<T> : ITypeParser
    {
        /// <summary>
        /// Gets or sets if this type parser can parse inherited types.
        /// </summary>
        protected abstract bool CanParseInheritedTypes { get; set; }

        /// <inheritdoc />
        public bool CanParse(Type type)
        {
            if (CanParseInheritedTypes)
                return typeof(T).IsAssignableFrom(type);

            return typeof(T) == type;
        }

        /// <inheritdoc />
        public abstract Task<ITypeParserResult> ParseAsync(string value, IParameter parameter, CommandContext context);
    }
}