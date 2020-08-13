using System;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.TypeParsers
{
    /// <summary>
    /// Defines a type argument parser for the specified type.
    /// </summary>
    public interface ITypeParser
    {
        /// <summary>
        /// Define if this type parser can parse the specified type.
        /// </summary>
        /// <param name="type">The type to be parsed.</param>
        /// <returns><see langword="true" /> if the specified type can be parsed.</returns>
        bool CanParse(Type type);

        /// <summary>
        /// Asynchronously parse a raw value to the specified type.
        /// </summary>
        /// <param name="value">The raw argument value.</param>
        /// <param name="parameter">The specified param to be parsed.</param>
        /// <param name="context">The current command execution context.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="ITypeParserResult{T}" />.</returns>
        Task<ITypeParserResult> ParseAsync(string value, IParameter parameter, CommandContext context);
    }

    /// <inheritdoc />
    public interface ITypeParser<T> : ITypeParser
    {
        /// <summary>
        /// Defines if this type can parse inherited types.
        /// </summary>
        bool CanParseInheritedTypes();

        /// <summary>
        /// Asynchronously parse a raw value to the specified type.
        /// </summary>
        /// <param name="value">The raw argument value.</param>
        /// <param name="parameter">The specified param to be parsed.</param>
        /// <param name="context">The current command execution context.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="ITypeParserResult{T}" />.</returns>
        new Task<ITypeParserResult<T>> ParseAsync(string value, IParameter parameter, CommandContext context);

        bool ITypeParser.CanParse(Type type)
        {
            if (CanParseInheritedTypes())
                return typeof(T).IsAssignableFrom(type);

            return typeof(T) == type;
        }

        async Task<ITypeParserResult> ITypeParser.ParseAsync(string value, IParameter parameter, CommandContext context)
            => await ParseAsync(value, parameter, context);
    }
}