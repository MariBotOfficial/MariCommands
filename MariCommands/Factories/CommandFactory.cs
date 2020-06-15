using System;
using System.Linq;
using System.Reflection;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ICommandServiceOptions _config;
        private readonly IParameterFactory _parameterFactory;

        /// <summary>
        /// Create a new instace of <see  cref="CommandFactory"/>.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public CommandFactory(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
            _parameterFactory = _provider.GetOrDefault<IParameterFactory>(new ParameterFactory(_provider));
        }


        /// <inheritdoc />
        public ICommandBuilder BuildCommand(IModuleBuilder module, MethodInfo methodInfo)
        {
            module.NotNull(nameof(module));
            methodInfo.NotNull(nameof(methodInfo));

            if (!IsCommand(module, methodInfo))
                throw new ArgumentException(nameof(methodInfo), $"{methodInfo.Name} is not a valid command.");

            return null;
        }

        /// <inheritdoc />
        public bool IsCommand(IModuleBuilder module, MethodInfo methodInfo)
        {
            var isValid = module.HasContent() &&
                          methodInfo.HasContent() &&
                          methodInfo.IsPublic &&
                          methodInfo.CustomAttributes.Any(a => a.AttributeType.IsEquivalentTo(typeof(CommandAttribute)));

            return isValid;
        }
    }
}