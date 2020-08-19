using System;

namespace MariCommands.Features
{
    /// <summary>
    /// Represents an exception handler.
    /// </summary>
    public interface IExceptionHandlerFeature
    {
        /// <summary>
        /// The error ocurried.
        /// </summary>
        Exception Error { get; }
    }
}