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
        public object Instantiate(CommandExecutionContext context)
        {
            var cmdContext = context.CommandContext;
            var module = cmdContext.Command.Module;
            var lifeTime = module.GetModuleLifetime(_config);

            var instance = _provider.GetService(module.Type);

            if (instance.HasContent())
            {
                SetNeedDispose(context, false);
                return instance;
            }

            if (lifeTime == ModuleLifetime.Transient)
            {
                SetNeedDispose(context, true);
                instance = GetTransient(module.Type);
            }
            else
            {
                SetNeedDispose(context, false);
                instance = GetSingleton(module.Type);
            }

            // TODO: Property Injection.

            return instance;
        }

        private object GetTransient(Type type)
            => ActivatorUtilities.CreateInstance(_provider, type);

        private object GetSingleton(Type type)
        {
            if (_instances.TryGetValue(type, out var instance))
                return instance;

            instance = ActivatorUtilities.CreateInstance(_provider, type);

            _instances.TryAdd(type, instance);

            return instance;
        }

        private void SetNeedDispose(CommandExecutionContext context, bool value)
        {
            if (context.Items.ContainsKey(ServiceUtils.NEED_DISPOSE))
            {
                context.Items[ServiceUtils.NEED_DISPOSE] = value;
            }
            else
            {
                context.Items.Add(ServiceUtils.NEED_DISPOSE, value);
            }
        }
    }
}