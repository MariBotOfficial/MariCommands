using System;
using System.Reflection;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class ParameterFactory : IParameterFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ICommandServiceOptions _config;

        /// <summary>
        /// Creates a new instance of <see cref="ParameterFactory" />.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public ParameterFactory(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
        }

        /// <inheritdoc />
        public IParameterBuilder BuildParameter(ICommandBuilder command, ParameterInfo parameterInfo)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsParameter(ICommandBuilder command, ParameterInfo parameterInfo)
        {
            var isValid =
                        command.HasContent() &&
                        parameterInfo.HasContent();

            return isValid;
        }
    }
}