using System;
using System.Threading.Tasks;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when a task is executing and the result is not returned yet.
    /// </summary>
    public class AsyncResult : IResult
    {
        private readonly TaskCompletionSource<IResult> _tsc;

        /// <summary>
        /// Creates a new instance of <see cref="AsyncResult" /> with the specified task completion source.
        /// </summary>
        /// <param name="tsc">The task completion source that will receive the result of the task.</param>
        public AsyncResult(TaskCompletionSource<IResult> tsc)
        {
            _tsc = tsc;
        }

        /// <inheritdoc />
        public bool Success => true;

        /// <inheritdoc />
        public string Reason => null;

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// Gets the original result of the task that was executing.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation within the result of the
        /// original executing task.</returns>
        public Task<IResult> GetResultAsync()
            => _tsc.Task;

        /// <summary>
        /// Creates a new instance of <see cref="AsyncResult" /> with the specified task completion source.
        /// </summary>
        /// <param name="tsc">The task completion source that will receive the result of the task.</param>
        /// <returns>Am <see cref="AsyncResult" /></returns>
        public static AsyncResult FromTsc(TaskCompletionSource<IResult> tsc)
            => new AsyncResult(tsc);
    }
}