using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Extensions;
using MariCommands.Providers;
using MariCommands.Utils;
using MariGlobals.Extensions;

namespace MariCommands.Factories
{
    /// <inheritdoc />
    internal sealed partial class CommandFactory : ICommandFactory
    {
        private readonly ICommandServiceOptions _config;
        private readonly IParameterFactory _parameterFactory;
        private readonly ICommandExecutorProvider _executorProvider;

        public CommandFactory(ICommandServiceOptions config, IParameterFactory parameterFactory, ICommandExecutorProvider executorProvider)
        {
            _config = config;
            _parameterFactory = parameterFactory;
            _executorProvider = executorProvider;
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
            var isAsync = GetIsAsync(methodInfo);
            var asyncResultType = GetAsyncResultType(methodInfo, isAsync);

            var builder = new CommandBuilder()
                                .WithMethodInfo(methodInfo)
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
                                .WithModule(module)
                                .WithIsAsync(isAsync)
                                .WithAsyncResultType(asyncResultType);

            var executor = GetExecutor(module, builder);

            builder.WithExecutor(executor);

            var parameters = GetParameters(builder);
            builder.WithParameters(parameters);

            return builder;
        }

        private ICommandExecutor GetExecutor(IModuleBuilder module, CommandBuilder builder)
            => _executorProvider.GetCommandExecutor(module, builder);

        private Type GetAsyncResultType(MethodInfo methodInfo, bool isAsync)
        {
            if (!isAsync || !methodInfo.ReturnType.IsGenericType)
                return null;

            return methodInfo.ReturnType.GetGenericArguments().FirstOrDefault();
        }

        private bool GetIsAsync(MethodInfo methodInfo)
        {
            var type = methodInfo.ReturnType;

            if (type == typeof(Task) && !type.IsGenericType)
                return true;

            if (type == typeof(ValueTask) && !type.IsGenericType)
                return true;

            if (!type.IsGenericType)
                return false;

            var genericDefinition = type.GetGenericTypeDefinition();

            if (genericDefinition == typeof(Task<>))
                return true;

            if (genericDefinition == typeof(ValueTask<>))
                return true;

            return false;
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
            var aliasAttr = methodInfo.GetAttribute<CommandAttribute>();

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
                          !methodInfo.IsStatic &&
                          !methodInfo.IsGenericMethod &&
                          methodInfo.CustomAttributes.Any(a => typeof(CommandAttribute).IsAssignableFrom(a.AttributeType));

            return isValid;
        }
    }
}