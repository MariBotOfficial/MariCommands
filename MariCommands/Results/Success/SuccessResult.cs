using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents a simple success result.
    /// </summary>
    public class SuccessResult : IResult
    {
        /// <inheritdoc />
        public bool Success => true;

        /// <inheritdoc />
        public string Reason => null;

        /// <inheritdoc />
        public Exception Exception => null;
    }
}