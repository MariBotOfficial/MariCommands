using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Results;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <summary>
    /// Represents a command context.
    /// </summary>
    public class CommandContext : IAsyncDisposable
    {
        private static readonly Func<IFeatureCollection, IItemsFeature> _newItemsFeature = f => new ItemsFeature();
        private static readonly Func<CommandContext, ICommandServiceProvidersFeature> _newServiceProvidersFeature = context => new CommandServicesFeature(context, context.ServiceScopeFactory);
        private static readonly Func<IFeatureCollection, IDisposablesFeature> _newDisposablesFeature = f => new DisposablesFeature();

        // We can use the default http request identifier feature.
        private static readonly Func<IFeatureCollection, IHttpRequestIdentifierFeature> _newHttpRequestIdentifierFeature = f => new HttpRequestIdentifierFeature();

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

        private IDisposablesFeature DisposablesFeature
            => _features.Fetch(ref _features.Cache.Disposables, _newDisposablesFeature);

        private IHttpRequestIdentifierFeature RequestIdentifierFeature =>
            _features.Fetch(ref _features.Cache.RequestIdentifier, _newHttpRequestIdentifierFeature);

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
        /// Gets or sets a unique identifier to represent this command request in trace logs.
        /// </summary>
        public string TraceIdentifier
        {
            get { return RequestIdentifierFeature.TraceIdentifier; }
            set { RequestIdentifierFeature.TraceIdentifier = value; }
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
            await DisposablesFeature.DisposeAsync();

            _features = default;
        }

        /// <summary>
        /// Register a disposable object for dispose after the command execution.
        /// </summary>
        /// <param name="disposable">The object to be disposed.</param>
        public void RegisterForDispose(IDisposable disposable)
            => DisposablesFeature.RegisterForDispose(disposable);

        /// <summary>
        /// Register a disposable object for dispose after the command execution.
        /// </summary>
        /// <param name="asyncDisposable">The object to be disposed.</param>
        public void RegisterForDisposeAsync(IAsyncDisposable asyncDisposable)
            => DisposablesFeature.RegisterForDisposeAsync(asyncDisposable);

        private static IFeatureCollection ContextDisposed()
        {
            ThrowContextDisposed();
            return null;
        }

        private static void ThrowContextDisposed()
        {
            throw new ObjectDisposedException(nameof(CommandContext), $"Command execution has finished and {nameof(CommandContext)} disposed.");
        }

        struct FeatureInterfaces
        {
            public IItemsFeature Items;

            public ICommandServiceProvidersFeature ServiceProviders;

            public IDisposablesFeature Disposables;

            public IHttpRequestIdentifierFeature RequestIdentifier;
        }
    }
}