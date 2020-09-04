using System;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Invokers
{
    internal sealed class DefaultModuleInvoker : IModuleInvoker
    {
        private readonly ObjectFactory _objectFactory;

        private DefaultModuleInvoker(ObjectFactory objectFactory)
        {
            _objectFactory = objectFactory;
        }

        public object CreateInstance(IServiceProvider provider)
            => _objectFactory(provider, new object[0]);

        public static DefaultModuleInvoker Create(Type type)
        {
            var factory = ActivatorUtilities.CreateFactory(type, new Type[0]);

            return new DefaultModuleInvoker(factory);
        }
    }
}