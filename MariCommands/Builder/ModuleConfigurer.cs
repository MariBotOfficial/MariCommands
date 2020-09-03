using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MariCommands.Factories;
using Microsoft.Extensions.Options;

namespace MariCommands.Builder
{
    /// <inheritdoc />
    public class ModuleConfigurer : IModuleConfigurer
    {
        private readonly IModuleCache _moduleCache;
        private readonly IModuleFactory _moduleFactory;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleConfigurer" />
        /// </summary>
        /// <param name="options">The current command app options.</param>
        /// <param name="moduleCache">The module cache to save the added modules.</param>
        /// <param name="moduleFactory">The factory to create modules.</param>
        public ModuleConfigurer(IOptions<MariCommandsOptions> options, IModuleCache moduleCache, IModuleFactory moduleFactory)
        {
            Options = options.Value;
            _moduleCache = moduleCache;
            _moduleFactory = moduleFactory;
        }

        /// <inheritdoc />
        public MariCommandsOptions Options { get; }

        /// <inheritdoc />
        public IModule AddModule(IModule module)
        {
            _moduleCache.AddModule(module);

            return module;
        }

        /// <inheritdoc />
        public IModule AddModule(Type type)
        {
            if (!_moduleFactory.IsModule(type))
                throw new InvalidOperationException($"Cannot add this module because it is not a valid module definition.");

            var moduleBuilder = _moduleFactory.BuildModule(null, type);

            var module = moduleBuilder.Build(null);

            _moduleCache.AddModule(module);

            return module;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IModule> AddModules(Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            var validTypes = types
                                .Where(a => _moduleFactory.IsModule(a))
                                .ToList();

            var modules = ImmutableArray.CreateBuilder<IModule>(validTypes.Count);

            foreach (var type in validTypes)
            {
                var builder = _moduleFactory.BuildModule(null, type);

                var module = builder.Build(null);

                modules.Add(module);
            }

            return modules.MoveToImmutable();
        }

        /// <inheritdoc />
        public void RemoveModule(IModule module)
            => _moduleCache.RemoveModule(module);
    }
}