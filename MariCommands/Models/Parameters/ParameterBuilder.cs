using System;
using System.Collections.Generic;

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
        public Type Type { get; private set; }

        /// <inheritdoc />
        public ICommandBuilder Command { get; private set; }
    }
}