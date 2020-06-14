using System;
using System.Collections.Generic;
using System.Reflection;

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
        public RunMode RunMode { get; private set; }

        /// <inheritdoc />
        public bool IgnoreExtraArgs { get; private set; }

        /// <inheritdoc />
        public Type ArgumentParserType { get; private set; }

        /// <inheritdoc />
        public string Separator { get; private set; }

        /// <inheritdoc />
        public StringComparison? Comparison { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Aliases { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<Attribute> Attributes { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IParameter> Parameters { get; private set; }

        /// <inheritdoc />
        public bool IsEnabled { get; private set; }

        /// <inheritdoc />    
        public MethodInfo Method { get; private set; }
    }
}