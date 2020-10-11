using System;
using System.Diagnostics;
using MariCommands.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Filters
{
    /// <summary>
    /// A command filter that finds another command filter in an <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Primarily used in <see cref="M:CommandFilterCollection.AddService"/> calls.
    /// </para>
    /// <para>
    /// Similar to the <see cref="TypeCommandFilterAttribute"/> in that both use constructor injection. Use
    /// <see cref="TypeCommandFilterAttribute"/> instead if the filter is not itself a service.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [DebuggerDisplay("ServiceCommandFilter: Type={ServiceType} Order={Order}")]
    public class ServiceCommandFilterAttribute : Attribute, ICommandFilterFactory, IOrderedCommandFilter
    {
        /// <summary>
        /// Creates a new instance of <see cref="ServiceCommandFilterAttribute" />.
        /// </summary>
        /// <param name="implementationType">The <see cref="Type" /> of the command filter to find.</param>
        public ServiceCommandFilterAttribute(Type implementationType)
        {
            implementationType.NotNull(nameof(implementationType));

            ImplementationType = implementationType;
        }

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public int Order { get; set; }

        /// <inheritdoc />
        public ICommandFilter CreateInstance(IServiceProvider serviceProvider)
        {
            serviceProvider.NotNull(nameof(serviceProvider));

            var filter = (ICommandFilter)serviceProvider.GetRequiredService(ImplementationType);

            if (filter is ICommandFilterFactory commandFilterFactory)
                filter = commandFilterFactory.CreateInstance(serviceProvider);

            return filter;
        }
    }
}