using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands.Filters
{
    internal sealed class FilterProvider : IFilterProvider
    {
        private readonly IEnumerable<IFilterFactory> _filterFactories;

        public FilterProvider(IEnumerable<IFilterFactory> filterFactories)
        {
            _filterFactories = filterFactories;
        }

        public Task InvokeFiltersAsync<TContext, TFilter>(TContext context)
            where TContext : IFilterContext
            where TFilter : ICommandFilter
        {
            var filterFactories = _filterFactories ?? Array.Empty<IFilterFactory>();
            var filterType = typeof(TFilter);
            IFilterFactory selectedFilterFactory = null;

            foreach (var filterFactory in filterFactories)
            {
                if (filterFactory.CanInvoke(context, filterType))
                {
                    selectedFilterFactory = filterFactory;
                    break;
                }
            }

            if (selectedFilterFactory.HasNoContent())
                return Task.CompletedTask;

            var buildedDelegate = selectedFilterFactory.GetFiltersDelegate();

            return (Task)buildedDelegate.DynamicInvoke(context);
        }
    }
}