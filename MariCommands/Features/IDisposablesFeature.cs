using System;

namespace MariCommands.Features
{
    /// <summary>
    /// A feature to register objects to dispose.
    /// </summary>
    public interface IDisposablesFeature : IAsyncDisposable
    {
        /// <summary>
        /// Register a disposable object for dispose after the command execution.
        /// </summary>
        /// <param name="disposable">The object to be disposed.</param>
        void RegisterForDispose(IDisposable disposable);

        /// <summary>
        /// Register a disposable object for dispose after the command execution.
        /// </summary>
        /// <param name="asyncDisposable">The object to be disposed.</param>
        void RegisterForDisposeAsync(IAsyncDisposable asyncDisposable);
    }
}