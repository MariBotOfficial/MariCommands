using System;

namespace MariCommands
{
    /// <summary>
    /// Represents a result for any execution proccess.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// When this result is success.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// The reason of the error.
        /// </summary>
        string Reason { get; }

        /// <summary>
        /// An exception represeting an error.
        /// </summary>
        Exception Exception { get; }
    }
}