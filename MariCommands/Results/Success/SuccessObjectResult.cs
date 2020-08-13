using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when a result is success but has an object as result.
    /// </summary>
    public class SuccessObjectResult : SuccessResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="SuccessObjectResult" /> with the specified object result.
        /// </summary>
        /// <param name="value">The object result.</param>
        public SuccessObjectResult(object value)
        {
            Value = value;
        }

        /// <summary>
        /// The object result.
        /// </summary>
        public object Value { get; }
    }
}