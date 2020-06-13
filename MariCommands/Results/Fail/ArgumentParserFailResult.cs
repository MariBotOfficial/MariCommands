using System;

namespace MariCommands
{
    /// <inheritdoc />
    public class ArgumentParserFailResult : ArgumentParserResult
    {
        /// <inheritdoc />
        public ArgumentParserFailResult(string reason) : base(reason)
        {
        }

        /// <inheritdoc />
        public ArgumentParserFailResult(Exception exception) : base(exception)
        {
        }
    }
}