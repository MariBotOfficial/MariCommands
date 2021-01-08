using System;
using System.Diagnostics;
using System.Linq;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Filters
{
    /// <summary>
    /// A filter that creates another filter of type <see cref="ImplementationType"/>, retrieving missing constructor
    /// arguments from dependency injection if available there.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Primarily used in <see cref="M:CommandFilterCollection.Add"/> calls.
    /// </para>
    /// <para>
    /// Similar to the <see cref="ServiceCommandFilterFactory"/> in that both use constructor injection. Use
    /// <see cref="ServiceCommandFilterFactory"/> instead if the filter is itself a service.
    /// </para>
    /// </remarks>
    [DebuggerDisplay("TypeCommandFilter: Type={ImplementationType} Order={Order}")]
    public class TypeCommandFilterFactory : ICommandFilterFactory, IOrderedCommandFilter
    {
        private ObjectFactory _factory;

        /// <summary>
        /// Creates a new instance of <see cref="TypeCommandFilterFactory" />.
        /// </summary>
        /// <param name="implementationType">The <see cref="Type" /> of the command filter to find.</param>
        public TypeCommandFilterFactory(Type implementationType)
        {
            implementationType.NotNull(nameof(implementationType));

            ImplementationType = implementationType;
        }

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the non-service arguments to pass to the <see cref="ImplementationType"/> constructor.
        /// </summary>
        /// <remarks>
        /// Service arguments are found in the dependency injection container i.e. this filter supports constructor
        /// injection in addition to passing the given <see cref="Arguments"/>.
        /// </remarks>
        public object[] Arguments { get; set; }

        /// <inheritdoc />
        public ICommandFilter CreateInstance(IServiceProvider serviceProvider)
        {
            serviceProvider.NotNull(nameof(serviceProvider));

            if (_factory.HasNoContent())
            {
                var argsTypes = Arguments
                                    ?.Select(a => a.GetType())
                                    ?.ToArray();

                _factory = ActivatorUtilities.CreateFactory(ImplementationType, argsTypes ?? Type.EmptyTypes);
            }

            var filter = (ICommandFilter)_factory(serviceProvider, Arguments);

            if (filter is ICommandFilterFactory commandFilterFactory)
                filter = commandFilterFactory.CreateInstance(serviceProvider);

            return filter;
        }
    }
}