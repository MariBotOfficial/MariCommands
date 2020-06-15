using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class ModuleFactory : IModuleFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ICommandServiceOptions _config;
        private readonly ICommandFactory _commandFactory;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleFactory" />.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public ModuleFactory(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
            _commandFactory = _provider.GetOrDefault<ICommandFactory>(new CommandFactory(_provider));
        }

        /// <inheritdoc />
        public IModuleBuilder BuildModule(IModuleBuilder parent, Type type)
        {
            type.NotNull(nameof(type));

            if (!IsModule(type))
                throw new ArgumentException(nameof(type), $"{type.FullName} is not a valid module.");

            var name = GetName(type);
            var description = GetDescription(type);
            var remarks = GetRemarks(type);
            var runMode = GetRunMode(type);
            var ignoreExtraArgs = GetIgnoreExtraArgs(type);
            var lifeTime = GetLifeTime(type);
            var argumentParserType = GetArgumentParserType(type);
            var multiMatchHandling = GetMultiMatch(type);
            var separator = GetSeparator(type);
            var alias = GetAlias(type);
            var enabled = GetEnabled(type);
            var attributes = GetAttributes(type);
            var preconditions = GetPreconditions(attributes);

            var builder = new ModuleBuilder()
                                .WithType(type)
                                .WithName(name)
                                .WithDescription(description)
                                .WithRemarks(remarks)
                                .WithRunMode(runMode)
                                .WithIgnoreExtraArgs(ignoreExtraArgs)
                                .WithLifeTime(lifeTime)
                                .WithArgumentParserType(argumentParserType)
                                .WithMultiMatch(multiMatchHandling)
                                .WithSeparator(separator)
                                .WithAlias(alias)
                                .WithEnabled(enabled)
                                .WithAttributes(attributes)
                                .WithPreconditions(preconditions)
                                .WithParent(parent);

            var subModules = GetSubModules(builder, type);
            var commands = GetCommands(builder);

            builder.WithSubmodules(subModules);
            builder.WithCommands(commands);

            return builder;
        }

        private IEnumerable<ICommandBuilder> GetCommands(IModuleBuilder builder)
        {
            var commandTypes = builder.Type.GetMethods()
                                            .Where(a => _commandFactory.IsCommand(builder, a))
                                            .ToList();

            var commands = ImmutableArray.CreateBuilder<ICommandBuilder>(commandTypes.Count);

            foreach (var commandType in commandTypes)
            {
                var commandBuilder = _commandFactory.BuildCommand(builder, commandType);

                commands.Add(commandBuilder);
            }

            return commands.MoveToImmutable();
        }

        private IEnumerable<IModuleBuilder> GetSubModules(IModuleBuilder parent, Type type)
        {
            var subModuleTypes = type.GetNestedTypes()
                                        .Where(a => IsSubModule(type))
                                        .ToList();

            var subModules = ImmutableArray.CreateBuilder<IModuleBuilder>(subModuleTypes.Count);

            foreach (var subModuleType in subModuleTypes)
            {
                var subModuleBuilder = BuildModule(parent, type);

                subModules.Add(subModuleBuilder);
            }

            return subModules.MoveToImmutable();
        }

        private bool GetEnabled(Type type)
        {
            var disableAttr = type.GetAttribute<DisabledAttribute>();

            if (disableAttr.HasContent())
                return false;

            return true;
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions(IEnumerable<Attribute> attributes)
        {
            return attributes
                    .Where(a => a.GetType().IsEquivalentTo(typeof(PreconditionAttribute)))
                    .Select(a => a as PreconditionAttribute)
                    .ToList();
        }

        private IEnumerable<Attribute> GetAttributes(Type type)
        {
            return type.GetCustomAttributes();
        }

        private IEnumerable<string> GetAlias(Type type)
        {
            var aliasAttr = type.GetAttribute<GroupAttribute>();

            if (aliasAttr.HasContent())
                return aliasAttr.Aliases;

            return null;
        }

        private string GetSeparator(Type type)
        {
            var separatorAttr = type.GetAttribute<SeparatorAttribute>();

            if (separatorAttr.HasContent())
                return separatorAttr.Value;

            return _config.Separator;
        }

        private MultiMatchHandling GetMultiMatch(Type type)
        {
            var multiMatchAttr = type.GetAttribute<MultiMatchHandlingAttribute>();

            if (multiMatchAttr.HasContent())
                return multiMatchAttr.Value;

            return _config.MatchHandling;
        }

        private Type GetArgumentParserType(Type type)
        {
            var argumentParserAttr = type.GetAttribute<ArgumentParserAttribute>();

            // Can be null, if null we just will use the default or the injected in dependency.
            return argumentParserAttr?.Value;
        }

        private ModuleLifetime GetLifeTime(Type type)
        {
            var lifeTimeAttr = type.GetAttribute<LifetimeAttribute>();

            if (lifeTimeAttr.HasContent())
                return lifeTimeAttr.Value;

            return _config.ModuleLifetime;
        }

        private bool GetIgnoreExtraArgs(Type type)
        {
            var ignoreArgsAttr = type.GetAttribute<IgnoreExtraArgsAttribute>();

            if (ignoreArgsAttr.HasContent())
                return ignoreArgsAttr.Value;

            return _config.IgnoreExtraArgs;
        }

        private RunMode GetRunMode(Type type)
        {
            var runModeAttr = type.GetAttribute<RunModeAttribute>();

            if (runModeAttr.HasContent())
                return runModeAttr.Value;

            return _config.RunMode;
        }

        private string GetRemarks(Type type)
        {
            var remarksAttr = type.GetAttribute<RemarksAttribute>();

            if (remarksAttr.HasContent())
                return remarksAttr.Value;

            return string.Empty;
        }

        private string GetDescription(Type type)
        {
            var descriptionAttr = type.GetAttribute<DescriptionAttribute>();

            if (descriptionAttr.HasContent())
                return descriptionAttr.Value;

            return string.Empty;
        }

        private string GetName(Type type)
        {
            var nameAttr = type.GetAttribute<NameAttribute>();

            if (nameAttr.HasContent())
                return nameAttr.Value;

            return type.Name;
        }

        /// <inheritdoc />
        public bool IsModule(Type type)
        {
            var isValid =
                type.HasContent() &&
                !type.IsNested &&
                type.IsEquivalentTo(typeof(IModuleBase<>)) &&
                !type.CustomAttributes
                        .Any(a => a.AttributeType.IsEquivalentTo(typeof(DontLoadAttribute)));

            return isValid;
        }

        /// <inheritdoc />
        public bool IsSubModule(Type type)
        {
            var isValid =
                type.HasContent() &&
                type.IsNested &&
                type.CustomAttributes
                    .Any(a => a.AttributeType.IsEquivalentTo(typeof(GroupAttribute))) &&
                !type.CustomAttributes
                    .Any(a => a.AttributeType.IsEquivalentTo(typeof(DontLoadAttribute)));

            return isValid;
        }
    }
}