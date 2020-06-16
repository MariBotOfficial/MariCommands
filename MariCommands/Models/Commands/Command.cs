using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;

namespace MariCommands
{
    /// <inheritdoc />
    public class Command : ICommand
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public string Remarks { get; }

        /// <inheritdoc />
        public int Priority { get; }

        /// <inheritdoc />
        public RunMode? RunMode { get; }

        /// <inheritdoc />
        public bool? IgnoreExtraArgs { get; }

        /// <inheritdoc />
        public Type ArgumentParserType { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Aliases { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IParameter> Parameters { get; }

        private bool _isEnabled;

        /// <summary>
        /// Creates a new instance of <see cref="Command" />
        /// </summary>
        /// <param name="builder">The builder for this command.</param>
        /// <param name="module">The module of this command.</param>
        public Command(ICommandBuilder builder, IModule module)
        {
            Validate(builder, module);

            Name = builder.Name;
            Description = builder.Description;
            Remarks = builder.Remarks;
            Priority = builder.Priority;
            RunMode = builder.RunMode;
            IgnoreExtraArgs = builder.IgnoreExtraArgs;
            ArgumentParserType = builder.ArgumentParserType;
            Aliases = builder.Aliases;
            Attributes = builder.Attributes;
            Preconditions = builder.Preconditions;
            _isEnabled = builder.IsEnabled;
            MethodInfo = builder.MethodInfo;
            Module = module;

            var parameters = ImmutableArray.CreateBuilder<IParameter>(builder.Parameters.Count);

            foreach (var parameterBuilder in builder.Parameters)
                parameters.Add(parameterBuilder.Build(this));

            Parameters = parameters.MoveToImmutable();

            CommandDelegate = builder.CommandDelegate;
        }

        private void Validate(ICommandBuilder builder, IModule module)
        {
            builder.NotNull(nameof(builder));
            module.NotNull(nameof(module));
            builder.Name.NotNullOrWhiteSpace(nameof(builder.Name));
            builder.MethodInfo.NotNull(nameof(builder.MethodInfo));
            builder.Aliases.NotNullOrEmpty(nameof(builder.Aliases));
            builder.CommandDelegate.NotNull(nameof(builder.CommandDelegate));
        }

        /// <inheritdoc />
        public bool IsEnabled
            => Module.IsEnabled && Volatile.Read(ref _isEnabled);

        /// <inheritdoc />
        public IModule Module { get; }

        /// <inheritdoc />
        public MethodInfo MethodInfo { get; }

        /// <inheritdoc />        
        public CommandDelegate CommandDelegate { get; }

        /// <inheritdoc />
        public void Disable()
            => Volatile.Write(ref _isEnabled, false);

        /// <inheritdoc />
        public void Enable()
            => Volatile.Write(ref _isEnabled, true);
    }
}