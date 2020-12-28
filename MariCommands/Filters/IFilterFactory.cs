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
        /// Determine if this factory can create a delegate for the specified <see cref="CommandFilterType" />.
        /// </summary>
        /// <param name="context">The current filter context.</param>
        /// <param name="filterType">The filter type to determine.</param>
        /// <returns><c>true</c> if this factory can create a delegate for the specified type, otherwise, <c>false</c>.</returns>
        bool CanInvoke(IFilterContext context, CommandFilterType filterType);

        /// <summary>
        /// Get a builded delegate that can process the filter context.
        /// </summary>
        /// <returns>A builded delegate that can process the filter context.</returns>
        Delegate GetFiltersDelegate();
    }

    /// <inheritdoc />
    public interface IFilterFactory<TFilter, TFilterDelegate> : IFilterFactory
        where TFilter : ICommandFilter
        where TFilterDelegate : Delegate
    {

        bool IFilterFactory.CanInvoke(IFilterContext context, CommandFilterType filterType)
        {
            // TODO: Utils/Helper logic, will be used for filter factory too.
            var currentFilterType = typeof(TFilter);

            if (filterType == CommandFilterType.Result && typeof(ICommandResultFilter).IsAssignableFrom(currentFilterType))
                return true;

            if (filterType == CommandFilterType.Exception && typeof(ICommandExceptionFilter).IsAssignableFrom(currentFilterType))
                return true;

            return false;
        }

        Delegate IFilterFactory.GetFiltersDelegate()
            => GetFiltersDelegate();

        /// <inheritdoc />
        new TFilterDelegate GetFiltersDelegate();
    }
}