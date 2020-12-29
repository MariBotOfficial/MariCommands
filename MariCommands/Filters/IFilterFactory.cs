using System;
using System.Collections.Generic;

namespace MariCommands.Filters
{
    /// <summary>
    /// A factory that can create a delegate for the specified filter type.
    /// </summary>
    public interface IFilterFactory
    {
        /// <summary>
        /// Determine if this factory can create a delegate for the specified filter type.
        /// </summary>
        /// <param name="context">The current filter context.</param>
        /// <param name="filterType">The filter type to determine.</param>
        /// <returns><c>true</c> if this factory can create a delegate for the specified type, otherwise, <c>false</c>.</returns>
        bool CanInvoke(IFilterContext context, Type filterType);

        /// <summary>
        /// Get a builded delegate that can process the filter context.
        /// </summary>
        /// <returns>A builded delegate that can process the filter context.</returns>
        Delegate GetFiltersDelegate();

        /// <summary>
        /// Notify to this factory that the filters collection was changed.
        /// </summary>
        /// <param name="filters">The new filters to be used.</param>
        void FiltersDefinitionWasChanged(IEnumerable<ICommandFilter> filters);
    }

    /// <inheritdoc />
    public interface IFilterFactory<TFilter, TFilterDelegate> : IFilterFactory
        where TFilter : ICommandFilter
        where TFilterDelegate : Delegate
    {

        bool IFilterFactory.CanInvoke(IFilterContext context, Type filterType)
        {
            if (typeof(TFilter).IsAssignableFrom(filterType))
                return true;

            return false;
        }

        Delegate IFilterFactory.GetFiltersDelegate()
            => GetFiltersDelegate();

        /// <inheritdoc />
        new TFilterDelegate GetFiltersDelegate();
    }
}