using System;
using System.Threading.Tasks;
using MariCommands.Utils;
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

        private IServiceScope _scope;

        /// <inheritdoc />
        public IServiceProvider CommandServices
        {
            get
            {
                if (_commandServices.HasNoContent())
                {
                    if (_scopeFactory.HasNoContent())
                        throw new InvalidOperationException(
                            $"Can't create a scoped service provider without a scope factory." +
                            $"Please set one in {nameof(CommandContext.ServiceScopeFactory)}.");

                    _context.RegisterForDisposeAsync(this);
                    _scope = _scopeFactory.CreateScope();
                    _commandServices = _scope.ServiceProvider;
                }

                return _commandServices;
            }
            set
            {
                _commandServices = value;
            }
        }

        /// <inheritdoc />
        public void Dispose()
            => DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await MiddlewareUtils.SwitchDisposeAsync(_scope);

            _scope = null;
            _commandServices = null;
        }
    }
}