using System;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands
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
        /// <see cref="ITypeParserResult" />.</returns>
        Task<ITypeParserResult> ParseAsync(string value, IParameter parameter, CommandContext context);
    }
}