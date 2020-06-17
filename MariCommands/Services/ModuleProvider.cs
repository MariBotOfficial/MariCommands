using System;
using System.Collections.Concurrent;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <inheritdoc />
    public class ModuleProvider : IModuleProvider
    {
        private readonly IServiceProvider _provider;
        private readonly ConcurrentDictionary<Type, object> _instances;
        private readonly ICommandServiceOptions _config;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleProvider" />.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public ModuleProvider(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
            _instances = new ConcurrentDictionary<Type, object>();
        }


        /// <inheritdoc />
        public object Instantiate(CommandContext context)
        {
            var module = context.Command.Module;
            var lifeTime = module.GetModuleLifetime(_config);

            object instance;

            if (lifeTime == ModuleLifetime.Transient)
                instance = GetTransient(module.Type);
            else
                instance = GetSingleton(module.Type);

            // TODO: Property Injection.

            return instance;
        }

        private object GetTransient(Type type)
            => ActivatorUtilities.GetServiceOrCreateInstance(_provider, type);

        private object GetSingleton(Type type)
        {
            var instance = _provider.GetService(type);

            if (instance.HasContent())
                return instance;

            if (_instances.TryGetValue(type, out instance))
                return instance;

            instance = ActivatorUtilities.CreateInstance(_provider, type);

            _instances.TryAdd(type, instance);

            return instance;
        }
    }
}