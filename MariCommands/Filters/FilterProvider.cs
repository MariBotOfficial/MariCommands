using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    internal sealed class FilterProvider : IFilterProvider
    {
        private readonly MariCommandsOptions _options;
        private readonly IEnumerable<IFilterFactory> _filterFactories;
        private readonly ConcurrentDictionary<Type, Func<IFilterContext, Task>> _cachedDelegates;

        public FilterProvider(IEnumerable<IFilterFactory> filterFactories, IOptions<MariCommandsOptions> options)
        {
            _filterFactories = filterFactories;
            _cachedDelegates = new ConcurrentDictionary<Type, Func<IFilterContext, Task>>();

            _options = options.Value;

            if (_options?.Filters != null)
                _options.Filters.CollectionChanged += FiltersChanged;
        }

        private void FiltersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move)
            {
                // Always clear all cached delegates, i.e it will be rebuilded.
                _cachedDelegates.Clear();
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                NotifySpecificFilterFactories(e.NewItems.Cast<ICommandFilter>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                NotifySpecificFilterFactories(e.OldItems.Cast<ICommandFilter>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                NotifySpecificFilterFactories(e.NewItems.Cast<ICommandFilter>());
                NotifySpecificFilterFactories(e.OldItems.Cast<ICommandFilter>());
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                NotifyAllFilterFactories();
            }
        }

        private void NotifySpecificFilterFactories(IEnumerable<ICommandFilter> filtersChanged)
        {
            var types = filtersChanged
                            .Select(a =>
                            {
                                if (a is ICommandFilterFactory commandFilterFactory)
                                    return commandFilterFactory.ImplementationType;

                                return a.GetType();
                            })
                            .ToList();

            foreach (var filterFactory in _filterFactories)
            {
                foreach (var type in types)
                {
                    if (filterFactory.CanInvoke(type))
                    {
                        filterFactory.FiltersDefinitionWasChanged(_options.Filters);
                    }
                }
            }
        }

        private void NotifyAllFilterFactories()
        {
            foreach (var filterFactory in _filterFactories)
                filterFactory.FiltersDefinitionWasChanged(_options.Filters);
        }

        public Task InvokeFiltersAsync<TContext, TFilter>(TContext context)
            where TContext : IFilterContext
            where TFilter : ICommandFilter
        {
            var filterType = typeof(TFilter);

            if (_cachedDelegates.TryGetValue(filterType, out var proccessDelegate))
                return proccessDelegate(context);

            IFilterFactory selectedFilterFactory = null;

            foreach (var filterFactory in _filterFactories)
            {
                if (filterFactory.CanInvoke(filterType))
                {
                    selectedFilterFactory = filterFactory;
                    break;
                }
            }

            if (selectedFilterFactory.HasNoContent())
                return Task.CompletedTask;

            var filterDelegate = selectedFilterFactory.GetFiltersDelegate();

            CheckFilterDelegate(selectedFilterFactory, filterDelegate);

            var buildedDelegate = BuildDelegate(filterType, filterDelegate);

            return buildedDelegate(context);
        }

        private Func<IFilterContext, Task> BuildDelegate(Type filterType, Delegate filterDelegate)
        {
            var delegateInstance = Expression.Constant(filterDelegate.Target);
            var contextParameter = Expression.Parameter(typeof(IFilterContext));
            var instanceCast = Expression.Convert(contextParameter, filterDelegate.Method.GetParameters().First().ParameterType);
            var callExpression = Expression.Call(delegateInstance, filterDelegate.Method, instanceCast);

            var lambda = Expression.Lambda<Func<IFilterContext, Task>>(callExpression, contextParameter);
            var buildedDelegate = lambda.Compile();

            _cachedDelegates.TryAdd(filterType, buildedDelegate);

            return buildedDelegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckFilterDelegate(IFilterFactory filterFactory, Delegate filterDelegate)
        {
            if (filterDelegate.HasNoContent())
                ThrowInvalidDelegate(filterFactory, "the process delegate cannot be null.");

            if (!typeof(Task).IsAssignableFrom(filterDelegate.Method.ReturnType))
                ThrowInvalidDelegate(filterFactory, $"the process delegate should return a '{nameof(Task)}'.");

            if (!filterDelegate.Method.GetParameters().Any())
                ThrowInvalidDelegate(filterFactory, $"the process delegate should contains at least one parameter.");

            if (!typeof(IFilterContext).IsAssignableFrom(filterDelegate.Method.GetParameters().First().ParameterType))
                ThrowInvalidDelegate(filterFactory, $"the process delegate parameter should implements '{nameof(IFilterContext)}'.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowInvalidDelegate(IFilterFactory filterFactory, string errorMessage)
            => throw new InvalidOperationException(
                    $"Cannot proccess filters using filter factory {filterFactory.GetType().Name} because it returned a invalid delegate: {errorMessage}");
    }
}