using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MariCommands.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public class CommandContext : IAsyncDisposable
    {
        private readonly static Func<IFeatureCollection, IItemsFeature> _newItemsFeature = f => new ItemsFeature();
        private readonly static Func<CommandContext, ICommandServiceProvidersFeature> _newServiceProvidersFeature = context => new CommandServicesFeature(context, context.ServiceScopeFactory);

        private const string DISPOSABLES_KEY = "RegisteredForDispose";

        private const string DISPOSABLESASYNC_KEY = "RegisteredForDisposeAsync";

        private FeatureReferences<FeatureInterfaces> _features;

        /// <summary>
        /// Creates a new instance of the <see cref="CommandContext" />.
        /// </summary>
        public CommandContext()
            : this(new FeatureCollection())
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandContext" />.
        /// </summary>
        public CommandContext(IFeatureCollection features)
        {
            _features.Initalize(features);
        }

        private IItemsFeature ItemsFeature
            => _features.Fetch(ref _features.Cache.Items, _newItemsFeature);

        private ICommandServiceProvidersFeature ServiceProvidersFeature
            => _features.Fetch(ref _features.Cache.ServiceProviders, this, _newServiceProvidersFeature);

        /// <summary>
        /// Get the current features 
        /// </summary>
        public IFeatureCollection Features => _features.Collection ?? ContextDisposed();

        /// <summary>
        /// The currently command of this context.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// The alias used for this command.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// The raw arguments of this context.
        /// </summary>
        public string RawArgs { get; set; }

        /// <summary>
        /// The parsed arguments.
        /// </summary>
        public IReadOnlyCollection<object> Args { get; set; }

        /// <summary>
        /// The dependency container of this context.
        /// </summary>
        public IServiceProvider CommandServices
        {
            get => ServiceProvidersFeature.CommandServices;
            set => ServiceProvidersFeature.CommandServices = value;
        }

        /// <summary>
        /// The result of this execution context.
        /// </summary>
        public IResult Result { get; set; }

        /// <summary>
        /// A key/value collection to share data within the execution.
        /// </summary>
        public IDictionary<object, object> Items
        {
            get => ItemsFeature.Items;
            set => ItemsFeature.Items = value;
        }

        /// <summary>
        /// Get or sets the factory used to create services within a scope.
        /// </summary>
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            var disposables = (Items[DISPOSABLES_KEY] as ICollection<IDisposable>);

            foreach (var disposable in disposables)
                disposable.Dispose();

            var asyncDisposables = (Items[DISPOSABLESASYNC_KEY] as ICollection<IAsyncDisposable>);

            foreach (var asyncDisposable in asyncDisposables)
                await asyncDisposable.DisposeAsync();

            _features = default;
        }

        /// <summary>
        /// Register a disposable object for dispose after the request.
        /// </summary>
        /// <param name="disposable"></param>
        public void RegisterForDispose(IDisposable disposable)
        {
            if (Items.ContainsKey(DISPOSABLES_KEY))
            {
                var disposables = (Items[DISPOSABLES_KEY] as ICollection<IDisposable>);

                disposables.Add(disposable);
            }
            else
            {
                ICollection<IDisposable> disposables = new List<IDisposable>();
                disposables.Add(disposable);

                Items[DISPOSABLES_KEY] = disposables;
            }
        }

        /// <summary>
        /// Register a disposable object for dispose after the request.
        /// </summary>
        /// <param name="disposable"></param>
        public void RegisterForDisposeAsync(IAsyncDisposable disposable)
        {
            if (Items.ContainsKey(DISPOSABLES_KEY))
            {
                var disposables = (Items[DISPOSABLESASYNC_KEY] as ICollection<IAsyncDisposable>);

                disposables.Add(disposable);
            }
            else
            {
                ICollection<IAsyncDisposable> disposables = new List<IAsyncDisposable>();
                disposables.Add(disposable);

                Items[DISPOSABLESASYNC_KEY] = disposables;
            }
        }

        private static IFeatureCollection ContextDisposed()
        {
            ThrowContextDisposed();
            return null;
        }

        private static void ThrowContextDisposed()
        {
            throw new ObjectDisposedException(nameof(CommandContext), $"Command executio has finished and {nameof(CommandContext)} disposed.");
        }

        struct FeatureInterfaces
        {
            public IItemsFeature Items;

            public ICommandServiceProvidersFeature ServiceProviders;
        }
    }
}