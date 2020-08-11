using System.Collections.Generic;
using MariCommands.Results;

namespace MariCommands.Features
{
    internal sealed class ArgumentParserFeature : IArgumentParserFeature
    {
        private IDictionary<ICommandMatch, IReadOnlyDictionary<IParameter, object>> _commandArgs;

        public IDictionary<ICommandMatch, IReadOnlyDictionary<IParameter, object>> CommandArgs
        {
            get
            {
                if (_commandArgs == null)
                    _commandArgs = new Dictionary<ICommandMatch, IReadOnlyDictionary<IParameter, object>>();

                return _commandArgs;
            }
            set
            {
                _commandArgs = value;
            }
        }
    }
}