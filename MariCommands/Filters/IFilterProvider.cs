using System;
using System.Collections.Generic;

namespace MariCommands.Filters
{
    /// <summary>
    /// An interface that can provide filter for the specified type.
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// Determine if this filter provider can invoke the specified <see cref="CommandFilterType" />.
        /// </summary>
        /// <param name="context">The current filter context.</param>
        /// <param name="filterType">The filter type to determine.</param>
        /// <returns><c>true</c> if this provider can provide filters for the specified type; otherwise, <c>false</c>.</returns>
        bool CanInvoke(IFilterContext context, CommandFilterType filterType);

        /// <summary>
        /// Get a builded delegate that can process the filter context.
        /// </summary>
        /// <returns>A builded delegate that can process the filter context.</returns>
        Delegate GetFiltersDelegate();
    }

    /// <inheritdoc />
    public interface IFilterProvider<TFilter, TFilterDelegate> : IFilterProvider
        where TFilter : ICommandFilter
        where TFilterDelegate : Delegate
    {

        bool IFilterProvider.CanInvoke(IFilterContext context, CommandFilterType filterType)
        {
            // TODO: Utils/Helper logic, will be used for filter factory too.
            var currentFilterType = typeof(TFilter);

            if (filterType == CommandFilterType.Result && typeof(ICommandResultFilter).IsAssignableFrom(currentFilterType))
                return true;

            if (filterType == CommandFilterType.Exception && typeof(ICommandExceptionFilter).IsAssignableFrom(currentFilterType))
                return true;

            return false;
        }

        Delegate IFilterProvider.GetFiltersDelegate()
            => GetFiltersDelegate();

        /// <inheritdoc />
        new TFilterDelegate GetFiltersDelegate();
    }
}