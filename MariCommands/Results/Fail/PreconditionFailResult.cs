using System;

namespace MariCommands
{
    /// <inheritdoc />
    public class PreconditionFailResult : PreconditionResult
    {
        /// <inheritdoc />
        public PreconditionFailResult(string reason) : base(reason)
        {
        }

        /// <inheritdoc />
        public PreconditionFailResult(Exception exception) : base(exception)
        {
        }
    }
}