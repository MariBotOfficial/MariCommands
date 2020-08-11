using System.Collections.Generic;
using MariCommands.Results;

namespace MariCommands.Features
{
    internal sealed class ArgumentParserFeature : IArgumentParserFeature
    {
        private IDictionary<ICommandMatch, object[]> _commandArgs;

        public IDictionary<ICommandMatch, object[]> CommandArgs
        {
            get
            {
                if (_commandArgs == null)
                    _commandArgs = new Dictionary<ICommandMatch, object[]>();

                return _commandArgs;
            }
            set
            {
                _commandArgs = value;
            }
        }
    }
}