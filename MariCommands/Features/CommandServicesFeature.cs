using System;
using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Features
{
    /// <inheritdoc />
    internal sealed class CommandServicesFeature : ICommandServiceProvidersFeature, IAsyncDisposable, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CommandContext _context;

        public CommandServicesFeature(CommandContext context, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _context = context;
        }

        private IServiceProvider _commandServices;

        private bool _commandServicesSet;

        private IServiceScope _scope;

        /// <inheritdoc />
        public IServiceProvider CommandServices
        {
            get
            {
                if (!_commandServicesSet && _scopeFactory.HasContent())
                {
                    _context.RegisterForDisposeAsync(this);
                    _scope = _scopeFactory.CreateScope();
                    _commandServices = _scope.ServiceProvider;
                    _commandServicesSet = true;
                }

                return _commandServices;
            }
            set
            {
                _commandServices = value;
                _commandServicesSet = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
            => DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            switch (_scope)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }

            _scope = null;
            _commandServices = null;
        }
    }
}