using System;
using System.Threading;
using System.Threading.Tasks;
using MariCommands.Utils;
using Microsoft.Extensions.Hosting;

namespace MariCommands
{
    internal sealed class BackgroundStartupService : BackgroundService
    {
        private readonly IServiceProvider _appProvider;

        public BackgroundStartupService(IServiceProvider appProvider)
        {
            _appProvider = appProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ApplicationBuilderUtils.InitializeCommandApp(_appProvider);

            return Task.CompletedTask;
        }
    }
}