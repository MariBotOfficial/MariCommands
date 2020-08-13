using System;
using System.Collections.Generic;
using MariCommands.Builder;

namespace MariCommands.Factories
{
    /// <summary>
    /// An factory that can create an <see cref="ICommandApplicationBuilder" />.
    /// </summary>
    public interface ICommandApplicationBuilderFactory
    {
        /// <summary>
        /// Create a new <see cref="ICommandApplicationBuilder" />.
        /// </summary>
        /// <param name="properties">The properties of this builder.</param>
        /// <param name="provider">The current application dependency container.</param>
        /// <returns>A new instance of <see cref="ICommandApplicationBuilder" />.</returns>
        ICommandApplicationBuilder Create(IDictionary<string, object> properties, IServiceProvider provider);
    }
}