using System;
using MariCommands.TypeParsers;

namespace MariCommands.Providers
{
    /// <summary>
    /// An interface that can provider an <see cref="ITypeParser" /> to a specific type.
    /// </summary>
    public interface ITypeParserProvider
    {
        /// <summary>
        /// Provide an instance of <see cref="ITypeParser" /> to the specified type.
        /// </summary>
        /// <param name="type">The specified type.</param>
        /// <returns>An <see cref="ITypeParser" />.</returns>
        ITypeParser GetTypeParser(Type type);
    }
}