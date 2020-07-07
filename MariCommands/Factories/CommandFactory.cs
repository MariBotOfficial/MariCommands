using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public partial class CommandFactory : ICommandFactory
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

            var alias = GetAlias(methodInfo);
            var name = GetName(methodInfo, alias);
            var description = GetDescription(methodInfo);
            var remarks = GetRemarks(methodInfo);
            var priority = GetPriority(methodInfo);
            var runMode = GetRunMode(methodInfo);
            var ignoreExtraArgs = GetIgnoreExtraArgs(methodInfo);
            var argumentParserType = GetArgumentParserType(methodInfo);
            var attributes = GetAttributes(methodInfo);
            var preconditions = GetPreconditions(attributes);
            var enabled = GetEnabled(methodInfo);
            //var commandDelegate = GetCommandDelegate(methodInfo);

            var builder = new CommandBuilder()
                                .WithName(name)
                                .WithDescription(description)
                                .WithRemarks(remarks)
                                .WithPriority(priority)
                                .WithRunMode(runMode)
                                .WithIgnoreExtraArgs(ignoreExtraArgs)
                                .WithArgumentParserType(argumentParserType)
                                .WithAlias(alias)
                                .WithEnabled(enabled)
                                .WithAttributes(attributes)
                                .WithPreconditions(preconditions)
                                .WithModule(module);
            // .WithCommandDelegate(commandDelegate);

            var parameters = GetParameters(builder);
            builder.WithParameters(parameters);

            return builder;
        }

        private IEnumerable<IParameterBuilder> GetParameters(ICommandBuilder builder)
        {
            var parameterInfos = builder.MethodInfo.GetParameters()
                                            .Where(a => _parameterFactory.IsParameter(builder, a))
                                            .ToList();

            var parameters = ImmutableArray.CreateBuilder<IParameterBuilder>(parameterInfos.Count);

            foreach (var parameterInfo in parameterInfos)
            {
                var parameterBuild = _parameterFactory.BuildParameter(builder, parameterInfo);

                parameters.Add(parameterBuild);
            }

            return parameters.MoveToImmutable();
        }


        private bool GetEnabled(MethodInfo methodInfo)
        {
            var disableAttr = methodInfo.GetAttribute<DisabledAttribute>();

            if (disableAttr.HasContent())
                return false;

            return true;
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions(IEnumerable<Attribute> attributes)
        {
            return attributes
                    .Where(a => typeof(PreconditionAttribute).IsAssignableFrom(a.GetType()))
                    .Select(a => a as PreconditionAttribute)
                    .ToList();
        }

        private IEnumerable<Attribute> GetAttributes(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes();
        }

        private IEnumerable<string> GetAlias(MethodInfo methodInfo)
        {
            var aliasAttr = methodInfo.GetAttribute<GroupAttribute>();

            if (aliasAttr.HasContent())
                return aliasAttr.Aliases;

            return null;
        }

        private bool GetIgnoreExtraArgs(MethodInfo methodInfo)
        {
            var ignoreArgsAttr = methodInfo.GetAttribute<IgnoreExtraArgsAttribute>();

            if (ignoreArgsAttr.HasContent())
                return ignoreArgsAttr.Value;

            return _config.IgnoreExtraArgs;
        }

        private Type GetArgumentParserType(MethodInfo methodInfo)
        {
            var argumentParserAttr = methodInfo.GetAttribute<ArgumentParserAttribute>();

            // Can be null, if null we just will use the default or the injected in dependency.
            return argumentParserAttr?.Value;
        }

        private RunMode GetRunMode(MethodInfo methodInfo)
        {
            var runModeAttr = methodInfo.GetAttribute<RunModeAttribute>();

            if (runModeAttr.HasContent())
                return runModeAttr.Value;

            return _config.RunMode;
        }

        private int GetPriority(MethodInfo methodInfo)
        {
            var priorityAttr = methodInfo.GetAttribute<PriorityAttribute>();

            if (priorityAttr.HasContent())
                return priorityAttr.Value;

            return default;
        }
        private string GetRemarks(MethodInfo methodInfo)
        {
            var remarksAttr = methodInfo.GetAttribute<RemarksAttribute>();

            if (remarksAttr.HasContent())
                return remarksAttr.Value;

            return string.Empty;
        }

        private string GetDescription(MethodInfo methodInfo)
        {
            var descriptionAttr = methodInfo.GetAttribute<DescriptionAttribute>();

            if (descriptionAttr.HasContent())
                return descriptionAttr.Value;

            return string.Empty;
        }

        private string GetName(MethodInfo methodInfo, IEnumerable<string> alias)
        {
            var nameAttr = methodInfo.GetAttribute<NameAttribute>();

            if (nameAttr.HasContent())
                return nameAttr.Value;


            return alias.FirstOrDefault();
        }

        /// <inheritdoc />
        public bool IsCommand(IModuleBuilder module, MethodInfo methodInfo)
        {
            var isValid = module.HasContent() &&
                          methodInfo.HasContent() &&
                          methodInfo.IsPublic &&
                          methodInfo.CustomAttributes.Any(a => typeof(CommandAttribute).IsAssignableFrom(a.AttributeType));

            return isValid;
        }
    }
}