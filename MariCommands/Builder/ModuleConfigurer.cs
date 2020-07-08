using System;
using System.Collections.Generic;
using System.Reflection;

namespace MariCommands.Builder
{
    /// <inheritdoc />
    public class ModuleConfigurer : IModuleConfigurer
    {
        /// <summary>
        /// Creates a new instance of <see cref="ModuleConfigurer" />
        /// </summary>
        /// <param name="options">The current command app options.</param>
        public ModuleConfigurer(ICommandServiceOptions options)
        {
            Options = options;
        }

        /// <inheritdoc />
        public ICommandServiceOptions Options { get; }

        /// <inheritdoc />
        public IModule AddModule<T>(T type) where T : IModule
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IModule AddModule<T>() where T : IModule
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IModule AddModule(Type type)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IModule> AddModules(Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }
}