using System;

namespace MariCommands.Filters
{
    /// <summary>
    /// An interface for command filter that can create an instance of an executable command filter.
    /// </summary>
    public interface ICommandFilterFactory : ICommandFilter
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the command filter to create.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// Creates an instance of this command filter.
        /// </summary>
        /// <param name="serviceProvider">The command service provider.</param>
        /// <returns>An instace of this command filter.</returns>
        ICommandFilter CreateInstance(IServiceProvider serviceProvider);
    }
}