using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MariCommands
{
    /// <inheritdoc />
    public class ModuleBuilder : IModuleBuilder
    {
        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public string Remarks { get; private set; }

        /// <inheritdoc />
        public RunMode RunMode { get; private set; }

        /// <inheritdoc />
        public bool IgnoreExtraArgs { get; private set; }

        /// <inheritdoc />
        public ModuleLifetime ModuleLifetime { get; private set; }

        /// <inheritdoc />
        public Type ArgumentParserType { get; private set; }

        /// <inheritdoc />
        public MultiMatchHandling MultiMatchHandling { get; private set; }

        /// <inheritdoc />
        public string Separator { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Aliases { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; private set; }

        /// <inheritdoc />
        public Type Type { get; private set; }

        /// <inheritdoc />
        public bool IsEnabled { get; private set; }

        /// <inheritdoc />
        public IModuleBuilder Parent { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ICommandBuilder> Commands { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IModuleBuilder> Submodules { get; private set; }

        /// <summary>
        /// Sets the real type of this module.
        /// </summary>
        /// <param name="type">The type to be setted.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <param ref="type" /> must not be null.
        /// </exception>
        public ModuleBuilder WithType(Type type)
        {
            type.NotNull(nameof(type));

            Type = type;

            return this;
        }

        /// <summary>
        /// Sets the name for this module.
        /// </summary>
        /// <param name="name">The name to be setted.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <param ref="name" /> must not be null or white space.
        /// </exception>
        public ModuleBuilder WithName(string name)
        {
            name.NotNullOrWhiteSpace(nameof(name));

            Name = name;

            return this;
        }

        /// <summary>
        /// Sets the multi match handling for this module.
        /// </summary>
        /// <param name="multiMatchHandling">The multi match handling to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithMultiMatch(MultiMatchHandling multiMatchHandling)
        {
            MultiMatchHandling = multiMatchHandling;

            return this;
        }

        /// <summary>
        /// Sets all preconditions for this module.
        /// </summary>
        /// <param name="preconditions">The preconditions to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithPreconditions(IEnumerable<PreconditionAttribute> preconditions)
        {
            Preconditions = preconditions.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets the attributes for this module.
        /// </summary>
        /// <param name="attributes">The attributes to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithAttributes(IEnumerable<Attribute> attributes)
        {
            Attributes = attributes.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets if this module is enabled.
        /// </summary>
        /// <param name="enabled">The value to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithEnabled(bool enabled)
        {
            IsEnabled = enabled;

            return this;
        }

        /// <summary>
        /// Sets the aliases for this module.
        /// </summary>
        /// <param name="alias">The aliases to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithAlias(IEnumerable<string> alias)
        {
            Aliases = alias.ToImmutableHashSet();

            return this;
        }

        /// <summary>
        /// Sets the separator for this module.
        /// </summary>
        /// <param name="separator">The separator to be setted.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <param ref="separator" /> must not be null or white space.
        /// </exception>
        public ModuleBuilder WithSeparator(string separator)
        {
            separator.NotNullOrWhiteSpace(nameof(separator));

            Separator = separator;

            return this;
        }

        /// <summary>
        /// Sets a custom argument parser type for this module.
        /// </summary>
        /// <param name="argumentParserType">The custom argument parser type to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithArgumentParserType(Type argumentParserType)
        {
            // Can be null without problem.
            ArgumentParserType = argumentParserType;

            return this;
        }

        /// <summary>
        /// Sets the life time for this module.
        /// </summary>
        /// <param name="lifeTime">The life time to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithLifeTime(ModuleLifetime lifeTime)
        {
            ModuleLifetime = lifeTime;

            return this;
        }

        /// <summary>
        /// Sets the ignore extra args for this module.
        /// </summary>
        /// <param name="ignoreExtraArgs">The ignore extra args value to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithIgnoreExtraArgs(bool ignoreExtraArgs)
        {
            IgnoreExtraArgs = ignoreExtraArgs;

            return this;
        }

        /// <summary>
        /// Sets the run mode for this module.
        /// </summary>
        /// <param name="runMode">The run mode to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithRunMode(RunMode runMode)
        {
            RunMode = runMode;

            return this;
        }

        /// <summary>
        /// Sets any remarks for this module.
        /// </summary>
        /// <param name="remarks">The remarks to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithRemarks(string remarks)
        {
            Remarks = remarks;

            return this;
        }

        /// <summary>
        /// Sets the description for this module.
        /// </summary>
        /// <param name="description">The description to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithDescription(string description)
        {
            Description = description;

            return this;
        }

        /// <summary>
        /// Sets the parent module for this module.
        /// </summary>
        /// <param name="parent">The parent module to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithParent(IModuleBuilder parent)
        {
            Parent = parent;

            return this;
        }

        /// <summary>
        /// Sets the submodules for this module.
        /// </summary>
        /// <param name="submodules">The submodules to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithSubmodules(IEnumerable<IModuleBuilder> submodules)
        {
            Submodules = submodules.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets the commands for this module.
        /// </summary>
        /// <param name="commands">The commands to be setted.</param>
        /// <returns>The current builder.</returns>
        public ModuleBuilder WithCommands(IEnumerable<ICommandBuilder> commands)
        {
            Commands = commands.ToImmutableArray();

            return this;
        }

        /// <inheritdoc />
        public IModule Build(IModule parent)
        {
            // TODO: return new Module(this, parent);
            throw new NotImplementedException();
        }
    }
}