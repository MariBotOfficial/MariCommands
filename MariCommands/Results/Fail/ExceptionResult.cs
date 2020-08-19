using System;

namespace MariCommands.Results.Fail
{
    /// <summary>
    /// Represents when a execution fails.
    /// </summary>
    public class ExceptionResult : IResult
    {
        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => $"An unexpected error occurred {Exception.Message}";

        /// <inheritdoc />
        public Exception Exception { get; }
    }
}