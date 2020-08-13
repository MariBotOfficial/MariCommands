using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using MariCommands.Executors;
using MariCommands.Utils;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandBuilder : ICommandBuilder
    {
        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public string Remarks { get; private set; }

        /// <inheritdoc />
        public int Priority { get; private set; }

        /// <inheritdoc />
        public RunMode? RunMode { get; private set; }

        /// <inheritdoc />
        public bool? IgnoreExtraArgs { get; private set; }

        /// <inheritdoc />
        public Type ArgumentParserType { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Aliases { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IParameterBuilder> Parameters { get; private set; }

        /// <inheritdoc />
        public bool IsEnabled { get; private set; }

        /// <inheritdoc />
        public bool IsAsync { get; private set; }

        /// <inheritdoc />
        public Type AsyncResultType { get; private set; }

        /// <inheritdoc />
        public ICommandExecutor Executor { get; private set; }

        /// <inheritdoc />    
        public MethodInfo MethodInfo { get; private set; }

        /// <inheritdoc />
        public IModuleBuilder Module { get; private set; }

        /// <summary>
        /// Sets the parent module for this command.
        /// </summary>
        /// <param name="module">The parent module to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithModule(IModuleBuilder module)
        {
            Module = module;

            return this;
        }

        /// <summary>
        /// Sets all preconditions for this command.
        /// </summary>
        /// <param name="preconditions">The preconditions to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithPreconditions(IEnumerable<PreconditionAttribute> preconditions)
        {
            Preconditions = preconditions.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets the async result type of this command.
        /// </summary>
        /// <param name="asyncResultType">The async result type to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithAsyncResultType(Type asyncResultType)
        {
            AsyncResultType = asyncResultType;

            return this;
        }

        /// <summary>
        /// Sets the command executor for this command.
        /// </summary>
        /// <param name="executor">The executor to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithExecutor(ICommandExecutor executor)
        {
            executor.NotNull(nameof(executor));

            Executor = executor;

            return this;
        }

        /// <summary>
        /// Sets if the command is async.
        /// </summary>
        /// <param name="isAsync">The value to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithIsAsync(bool isAsync)
        {
            IsAsync = isAsync;

            return this;
        }

        /// <summary>
        /// Sets the attributes for this command.
        /// </summary>
        /// <param name="attributes">The attributes to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithAttributes(IEnumerable<Attribute> attributes)
        {
            Attributes = attributes.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets if this command is enabled.
        /// </summary>
        /// <param name="enabled">The value to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithEnabled(bool enabled)
        {
            IsEnabled = enabled;

            return this;
        }

        /// <summary>
        /// Sets the aliases for this command.
        /// </summary>
        /// <param name="alias">The aliases to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithAlias(IEnumerable<string> alias)
        {
            alias.NotNullOrEmpty(nameof(alias));

            Aliases = alias.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets a custom argument parser type for this command.
        /// </summary>
        /// <param name="argumentParserType">The custom argument parser type to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithArgumentParserType(Type argumentParserType)
        {
            // Can be null without problem.
            ArgumentParserType = argumentParserType;

            return this;
        }


        /// <summary>
        /// Sets the ignore extra args for this command.
        /// </summary>
        /// <param name="ignoreExtraArgs">The ignore extra args value to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithIgnoreExtraArgs(bool? ignoreExtraArgs)
        {
            IgnoreExtraArgs = ignoreExtraArgs;

            return this;
        }

        /// <summary>
        /// Sets the run mode for this command.
        /// </summary>
        /// <param name="priority">The run mode to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithPriority(int priority)
        {
            Priority = priority;

            return this;
        }

        /// <summary>
        /// Sets the run mode for this command.
        /// </summary>
        /// <param name="runMode">The run mode to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithRunMode(RunMode? runMode)
        {
            RunMode = runMode;

            return this;
        }

        /// <summary>
        /// Sets any remarks for this command.
        /// </summary>
        /// <param name="remarks">The remarks to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithRemarks(string remarks)
        {
            Remarks = remarks;

            return this;
        }

        /// <summary>
        /// Sets the description for this command.
        /// </summary>
        /// <param name="description">The description to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithDescription(string description)
        {
            Description = description;

            return this;
        }

        /// <summary>
        /// Sets the name for this command.
        /// </summary>
        /// <param name="name">The name to be setted.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <param ref="parameterInfo" /> must not be null or white space.
        /// </exception>
        public CommandBuilder WithName(string name)
        {
            name.NotNullOrWhiteSpace(nameof(name));

            Name = name;

            return this;
        }

        /// <summary>
        /// Sets the parameters for this command.
        /// </summary>
        /// <param name="parameters">The parameters to be setted.</param>
        /// <returns>The current builder.</returns>
        public CommandBuilder WithParameters(IEnumerable<IParameterBuilder> parameters)
        {
            Parameters = parameters.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets the real <see cref="MethodInfo" /> of this command.
        /// </summary>
        public CommandBuilder WithMethodInfo(MethodInfo methodInfo)
        {
            methodInfo.NotNull(nameof(methodInfo));

            MethodInfo = methodInfo;

            return this;
        }

        /// <inheritdoc />
        public ICommand Build(IModule module)
            => new Command(this, module);
    }
}