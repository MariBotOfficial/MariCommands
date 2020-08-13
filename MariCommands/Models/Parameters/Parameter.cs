using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using MariCommands.Utils;

namespace MariCommands
{
    /// <inheritdoc />
    public class Parameter : IParameter
    {
        /// <summary>
        /// Creates a new instance of <see cref="Parameter" />.
        /// </summary>
        /// <param name="builder">The builder of this parameter.</param>
        /// <param name="command">The command of this parameter.</param>
        public Parameter(IParameterBuilder builder, ICommand command)
        {
            Validate(builder, command);

            Name = builder.Name;
            Description = builder.Description;
            Remarks = builder.Remarks;
            IsRemainder = builder.IsRemainder;
            IsParams = builder.IsParams;
            IsOptional = builder.IsOptional;
            DefaultValue = builder.DefaultValue;
            TypeParserType = builder.TypeParserType;
            Preconditions = builder.Preconditions.ToImmutableArray();
            Attributes = builder.Attributes.ToImmutableArray();
            ParameterInfo = builder.ParameterInfo;
            Command = command;
        }

        private void Validate(IParameterBuilder builder, ICommand command)
        {
            builder.NotNull(nameof(builder));
            command.NotNull(nameof(command));
            builder.Name.NotNullOrWhiteSpace(nameof(builder.Name));
            builder.ParameterInfo.NotNull(nameof(builder.ParameterInfo));
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Description { get; }

        /// <inheritdoc />
        public string Remarks { get; }

        /// <inheritdoc />
        public bool IsRemainder { get; }

        /// <inheritdoc />
        public bool IsParams { get; }

        /// <inheritdoc />
        public bool IsOptional { get; }

        /// <inheritdoc />
        public object DefaultValue { get; }

        /// <inheritdoc />
        public Type TypeParserType { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<ParamPreconditionAttribute> Preconditions { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc />
        public ParameterInfo ParameterInfo { get; }

        /// <inheritdoc />
        public ICommand Command { get; }
    }
}