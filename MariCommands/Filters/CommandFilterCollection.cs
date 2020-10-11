using System;
using System.Collections.ObjectModel;
using MariCommands.Utils;

namespace MariCommands.Filters
{
    /// <summary>
    /// A collection of <see cref="ICommandFilter" />.
    /// </summary>
    public class CommandFilterCollection : Collection<ICommandFilter>
    {
        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <typeparam name="TFilterType">Type representing a <see cref="ICommandFilter"/>.</typeparam>
        /// <returns>A <see cref="ICommandFilter"/> representing the added type.</returns>
        /// <remarks>
        /// Filter instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a service as a filter.
        /// </remarks>
        public ICommandFilter Add<TFilterType>() where TFilterType : ICommandFilter
            => Add(typeof(TFilterType));

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <param name="filterType">Type representing a <see cref="ICommandFilter"/>.</param>
        /// <returns>A <see cref="ICommandFilter"/> representing the added type.</returns>
        /// <remarks>
        /// Filter instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a service as a filter.
        /// </remarks>
        public ICommandFilter Add(Type filterType)
        {
            filterType.NotNull(nameof(filterType));

            return Add(filterType, 0);
        }

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <typeparam name="TFilterType">Type representing a <see cref="ICommandFilter"/>.</typeparam>
        /// <param name="order">The order of the added filter.</param>
        /// <returns>A <see cref="ICommandFilter"/> representing the added type.</returns>
        /// <remarks>
        /// Filter instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a service as a filter.
        /// </remarks>
        public ICommandFilter Add<TFilterType>(int order) where TFilterType : ICommandFilter
            => Add(typeof(TFilterType), order);

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <param name="filterType">Type representing a <see cref="ICommandFilter"/>.</param>
        /// <param name="order">The order of the added filter.</param>
        /// <returns>A <see cref="ICommandFilter"/> representing the added type.</returns>
        /// <remarks>
        /// Filter instances will be created using
        /// <see cref="Microsoft.Extensions.DependencyInjection.ActivatorUtilities"/>.
        /// Use <see cref="AddService(Type)"/> to register a service as a filter.
        /// </remarks>
        public ICommandFilter Add(Type filterType, int order)
        {
            filterType.NotNull(nameof(filterType));

            CheckFilterDefinition(filterType);

            var filter = new TypeCommandFilterAttribute(filterType) { Order = order };

            base.Add(filter);

            return filter;
        }

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <typeparam name="TFilterType">Type representing a <see cref="ICommandFilter"/>.</typeparam>
        /// <returns>A <see cref="ICommandFilter"/> representing the added service type.</returns>
        /// <remarks>
        /// Filter instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register a service that will be created via
        /// type activation.
        /// </remarks>
        public ICommandFilter AddService<TFilterType>() where TFilterType : ICommandFilter
            => AddService(typeof(TFilterType));

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <param name="filterType">Type representing a <see cref="ICommandFilter"/>.</param>
        /// <returns>A <see cref="ICommandFilter"/> representing the added service type.</returns>
        /// <remarks>
        /// Filter instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register a service that will be created via
        /// type activation.
        /// </remarks>
        public ICommandFilter AddService(Type filterType)
        {
            filterType.NotNull(nameof(filterType));

            return AddService(filterType, 0);
        }

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <typeparam name="TFilterType">Type representing a <see cref="ICommandFilter"/>.</typeparam>
        /// <param name="order">The order of the added filter.</param>
        /// <returns>A <see cref="ICommandFilter"/> representing the added service type.</returns>
        /// <remarks>
        /// Filter instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register a service that will be created via
        /// type activation.
        /// </remarks>
        public ICommandFilter AddService<TFilterType>(int order) where TFilterType : ICommandFilter
            => AddService(typeof(TFilterType), order);

        /// <summary>
        /// Adds a type representing a <see cref="ICommandFilter"/>.
        /// </summary>
        /// <param name="filterType">Type representing a <see cref="ICommandFilter"/>.</param>
        /// <param name="order">The order of the added filter.</param>
        /// <returns>A <see cref="ICommandFilter"/> representing the added service type.</returns>
        /// <remarks>
        /// Filter instances will be created through dependency injection. Use
        /// <see cref="Add(Type)"/> to register a service that will be created via
        /// type activation.
        /// </remarks>
        public ICommandFilter AddService(Type filterType, int order)
        {
            filterType.NotNull(nameof(filterType));

            CheckFilterDefinition(filterType);

            var filter = new ServiceCommandFilterAttribute(filterType) { Order = order };

            base.Add(filter);

            return filter;
        }

        private void CheckFilterDefinition(Type filterType)
        {
            if (!typeof(ICommandFilter).IsAssignableFrom(filterType))
                throw new ArgumentException($"{nameof(filterType)} must derive from {nameof(ICommandFilter)}", nameof(filterType));
        }
    }
}