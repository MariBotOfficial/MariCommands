using System;
using System.Collections.Concurrent;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <inheritdoc />
    public class ModuleProvider : IModuleProvider
    {
        private readonly ConcurrentDictionary<Type, object> _instances;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleProvider" />.
        /// </summary>
        public ModuleProvider()
        {
            _instances = new ConcurrentDictionary<Type, object>();
        }


        /// <inheritdoc />
        public CommandExecutionRequest Instantiate(CommandExecutionContext context)
        {
            context.NotNull(nameof(context));

            var cmdContext = context.CommandContext;

            cmdContext.NotNull(nameof(context.CommandContext));
            cmdContext.Command.NotNull(nameof(context.CommandContext.Command));

            var module = cmdContext.Command.Module;

            module.NotNull(nameof(context.CommandContext.Command.Module));

            var provider = cmdContext.ServiceProvider ?? ServiceUtils.GetDefaultServiceProvider();
            var config = provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();

            var lifeTime = module.GetModuleLifetime(config);

            var instance = provider.GetService(module.Type);

            if (instance.HasContent())
            {
                return new CommandExecutionRequest(instance, context, false);
            }

            if (lifeTime == ModuleLifetime.Transient)
            {
                instance = GetTransient(module.Type, provider);
                return new CommandExecutionRequest(instance, context, true);
            }
            else
            {
                instance = GetSingleton(module.Type, provider);
                return new CommandExecutionRequest(instance, context, false);
            }

            // TODO: Property Injection.
        }

        private object GetTransient(Type type, IServiceProvider provider)
            => ActivatorUtilities.CreateInstance(provider, type);

        private object GetSingleton(Type type, IServiceProvider provider)
        {
            if (_instances.TryGetValue(type, out var instance))
                return instance;

            instance = ActivatorUtilities.CreateInstance(provider, type);

            _instances.TryAdd(type, instance);

            return instance;
        }
    }
}