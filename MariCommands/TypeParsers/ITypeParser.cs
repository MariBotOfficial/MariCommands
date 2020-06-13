using System.Threading.Tasks;

namespace MariCommands
{
    /// <summary>
    /// Defines a type argument parser for the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypeParser<T>
    {
        /// <summary>
        /// Asynchronously parse a raw value to the specified type.
        /// </summary>
        /// <param name="value">The raw argument value.</param>
        /// <param name="parameter">The specified param to be parsed.</param>
        /// <param name="context">The current command execution context.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="ITypeParserResult{T}" />.</returns>
        Task<ITypeParserResult<T>> ParseAsync(string value, IParameter parameter, ICommandContext context);
    }
}