using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    /// <summary>
    /// Base class abstraction for filter factories.
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TFilterDelegate"></typeparam>
    public abstract class BaseFilterFactory<TFilter, TFilterDelegate> : IFilterFactory<TFilter, TFilterDelegate>
        where TFilter : ICommandFilter
        where TFilterDelegate : Delegate
    {
        private TFilterDelegate _delegate;
        private IEnumerable<ICommandFilter> _filters;

        /// <summary>
        /// Gets or sets the current builded delegate.
        /// </summary>
        protected TFilterDelegate Delegate
        {
            get => _delegate;
            set => _delegate = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseFilterFactory{TFilter, TFilterDelegate}" />.
        /// </summary>
        /// <param name="filters">All filters to be used.</param>
        protected BaseFilterFactory(IEnumerable<ICommandFilter> filters)
        {
            _filters = filters;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BaseFilterFactory{TFilter, TFilterDelegate}" />.
        /// </summary>
        /// <param name="options">General options containing all registered filters.</param>
        protected BaseFilterFactory(IOptions<MariCommandsOptions> options)
        {
            _filters = options.Value.Filters;
        }

        /// <inheritdoc />
        public void FiltersDefinitionWasChanged(IEnumerable<ICommandFilter> filters)
        {
            _filters = filters;

            BuildDelegate();
        }

        /// <inheritdoc />
        public virtual TFilterDelegate GetFiltersDelegate()
        {
            if (Delegate.HasContent())
                return Delegate;

            BuildDelegate();

            return Delegate;
        }

        /// <summary>
        /// Send a request to the current factory to rebuild the delegate.
        /// </summary>
        protected abstract void BuildDelegate();

        /// <summary>
        /// Get all registered filters as factories for the specified <typeparamref name="TFilter" />.
        /// </summary>
        /// <returns>All registered filters as factories for the specified <typeparamref name="TFilter" />.</returns>
        protected virtual IEnumerable<ICommandFilterFactory> GetFilterFactories()
        {
            var filters = _filters ?? Array.Empty<ICommandFilter>();

            var filterFactories = filters
                                    .Where(a => a is ICommandFilterFactory filterFactory && typeof(TFilter).IsAssignableFrom(filterFactory.ImplementationType))
                                    .OfType<ICommandFilterFactory>()
                                    .ToList();

            filters = filters.Except(filterFactories);

            var remainingFilterFactories = filters
                                    .Where(a => typeof(TFilter).IsAssignableFrom(a.GetType()))
                                    .Select(a => new TypeCommandFilterFactory(a.GetType()))
                                    .ToList();

            filterFactories.AddRange(remainingFilterFactories);

            return filterFactories
                            .OrderBy(a => a is IOrderedCommandFilter orderedCommandFilter
                                                    ? orderedCommandFilter.Order
                                                    : 0
                            )
                            .ToList();
        }
    }
}