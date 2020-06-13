using System.Collections.Generic;

namespace MariCommands
{
    /// <inheritdoc />
    public class ArgumentParserSuccessResult : ArgumentParserResult
    {
        /// <inheritdoc />
        public ArgumentParserSuccessResult(IDictionary<IParameter, object> args) : base(args)
        {
        }
    }
}