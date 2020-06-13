using System;
using System.Collections.Generic;
using System.Reflection;

namespace MariCommands
{
    public class CommandBuilder : ICommandBuilder
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Remarks { get; private set; }

        public int Priority { get; private set; }

        public RunMode RunMode { get; private set; }

        public bool IgnoreExtraArgs { get; private set; }

        public Type ArgumentParserType { get; private set; }

        public string Separator { get; private set; }

        public StringComparison? Comparison { get; private set; }

        public IReadOnlyCollection<string> Aliases { get; private set; }

        public IReadOnlyCollection<Attribute> Attributes { get; private set; }

        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; private set; }

        public IReadOnlyCollection<IParameter> Parameters { get; private set; }

        public bool IsEnabled { get; private set; }

        public MethodInfo Method { get; private set; }
    }
}