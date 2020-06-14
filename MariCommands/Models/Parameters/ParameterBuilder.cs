using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace MariCommands
{
    /// <inheritdoc />
    public class ParameterBuilder : IParameterBuilder
    {
        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public string Remarks { get; private set; }

        /// <inheritdoc />
        public bool IsRemainder { get; private set; }

        /// <inheritdoc />
        public bool IsParams { get; private set; }

        /// <inheritdoc />
        public bool IsOptional { get; private set; }

        /// <inheritdoc />
        public object DefaultValue { get; private set; }

        /// <inheritdoc />
        public Type TypeParserType { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ParamPreconditionAttribute> Preconditions { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; private set; }

        /// <inheritdoc />
        public ParameterInfo ParameterInfo { get; private set; }

        /// <inheritdoc />
        public ICommandBuilder Command { get; private set; }

        /// <summary>
        /// Sets the <see cref="ParameterInfo" /> for this parameter.
        /// </summary>
        /// <param name="parameterInfo">The <see cref="ParameterInfo" /> to be setted.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <param ref="parameterInfo" /> must not be null.
        /// </exception>
        public ParameterBuilder WithParameterInfo(ParameterInfo parameterInfo)
        {
            parameterInfo.NotNull(nameof(parameterInfo));

            ParameterInfo = parameterInfo;

            return this;
        }

        /// <summary>
        /// Sets any remarks for this parameter.
        /// </summary>
        /// <param name="remarks">Any remarks to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithRemarks(string remarks)
        {
            Remarks = remarks;

            return this;
        }

        /// <summary>
        /// Sets a custom type parser type for this parameter.
        /// </summary>
        /// <param name="typeParserType">The custom type parser type to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithTypeParserType(Type typeParserType)
        {
            TypeParserType = typeParserType;

            return this;
        }

        /// <summary>
        /// Sets all preconditions for this parameter.
        /// </summary>
        /// <param name="preconditions">All preconditions to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithPreconditions(IEnumerable<ParamPreconditionAttribute> preconditions)
        {
            Preconditions = preconditions.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets all attributes for this parameter.
        /// </summary>
        /// <param name="attributes">All attributes to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithAttributes(IEnumerable<Attribute> attributes)
        {
            Attributes = attributes.ToImmutableArray();

            return this;
        }

        /// <summary>
        /// Sets the default value of this parameter.
        /// </summary>
        /// <param name="defaultValue">The default value to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithDefaultValue(object defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        /// <summary>
        /// Sets if this parameter is optional.
        /// </summary>
        /// <param name="isOptional">The optional value to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithOptional(bool isOptional)
        {
            IsOptional = isOptional;

            return this;
        }

        /// <summary>
        /// Sets if this parameter uses the <see langword="params" /> modifier.
        /// </summary>
        /// <param name="isParams">The params value to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithParams(bool isParams)
        {
            IsParams = isParams;

            return this;
        }

        /// <summary>
        /// Sets if this parameter is remainder.
        /// </summary>
        /// <param name="isRemainder">The remainder value to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithRemainder(bool isRemainder)
        {
            IsRemainder = isRemainder;

            return this;
        }

        /// <summary>
        /// Sets the description for this parameter.
        /// </summary>
        /// <param name="description">The description to be setted.</param>
        /// <returns>The current builder.</returns>
        public ParameterBuilder WithDescription(string description)
        {
            Description = description;

            return this;
        }

        /// <summary>
        /// Sets the name for this parameter.
        /// </summary>
        /// <param name="name">The name to be setted.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <param ref="parameterInfo" /> must not be null or white space.
        /// </exception>
        public ParameterBuilder WithName(string name)
        {
            name.NotNullOrWhiteSpace(nameof(name));

            Name = name;

            return this;
        }

        /// <inheritdoc />
        public IParameter Build(ICommand command)
            => new Parameter(this, command);
    }
}