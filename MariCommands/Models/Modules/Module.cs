using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using MariCommands.Utils;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class Module : IModule
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public string Remarks { get; }

        /// <inheritdoc />
        public RunMode? RunMode { get; }

        /// <inheritdoc />
        public bool? IgnoreExtraArgs { get; }

        /// <inheritdoc />
        public ModuleLifetime? ModuleLifetime { get; }

        /// <inheritdoc />
        public Type ArgumentParserType { get; }

        /// <inheritdoc />
        public MultiMatchHandling? MultiMatchHandling { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Aliases { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IModule> Submodules { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ICommand> Commands { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <inheritdoc />
        public IModule Parent { get; }

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public bool IsEnabled
        {
            get
            {
                if (Parent.HasContent())
                    return Parent.IsEnabled && Volatile.Read(ref _isEnabled);
                else
                    return Volatile.Read(ref _isEnabled);
            }
        }

        private bool _isEnabled;

        /// <summary>
        /// Creates a new instance of <see cref="Module" />.
        /// </summary>
        /// <param name="builder">The builder of this module.</param>
        /// <param name="parent">The parent module.</param>
        public Module(IModuleBuilder builder, IModule parent)
        {
            Validate(builder);

            Name = builder.Name;
            Description = builder.Description;
            Remarks = builder.Remarks;
            RunMode = builder.RunMode;
            IgnoreExtraArgs = builder.IgnoreExtraArgs;
            ModuleLifetime = builder.ModuleLifetime;
            ArgumentParserType = builder.ArgumentParserType;
            MultiMatchHandling = builder.MultiMatchHandling;
            Aliases = builder.Aliases.ToImmutableArray();
            Attributes = builder.Attributes.ToImmutableArray();
            Preconditions = builder.Preconditions.ToImmutableArray();
            Type = builder.Type;
            _isEnabled = builder.IsEnabled;
            Parent = parent;

            var subModules = ImmutableArray.CreateBuilder<IModule>(builder.Submodules.Count);

            foreach (var subModuleBuilder in builder.Submodules)
                subModules.Add(subModuleBuilder.Build(this));

            Submodules = subModules.MoveToImmutable();

            var commands = ImmutableArray.CreateBuilder<ICommand>(builder.Commands.Count);

            foreach (var commandBuilder in builder.Commands)
                commands.Add(commandBuilder.Build(this));

            Commands = commands.MoveToImmutable();
        }

        private void Validate(IModuleBuilder builder)
        {
            builder.NotNull(nameof(builder));
            builder.Type.NotNull(nameof(builder.Type));
            builder.Name.NotNullOrWhiteSpace(nameof(builder.Name));
        }

        /// <inheritdoc />
        public void Disable()
            => Volatile.Write(ref _isEnabled, false);

        /// <inheritdoc />
        public void Enable()
            => Volatile.Write(ref _isEnabled, true);
    }
}