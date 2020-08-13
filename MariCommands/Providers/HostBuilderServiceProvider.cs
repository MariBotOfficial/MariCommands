using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MariCommands.Providers
{
    internal sealed class HostBuilderServiceProvider : IServiceProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public HostBuilderServiceProvider(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IConfiguration))
                return _configuration;

            if (serviceType == typeof(IHostEnvironment))
                return _hostEnvironment;

            return null;
        }
    }
}