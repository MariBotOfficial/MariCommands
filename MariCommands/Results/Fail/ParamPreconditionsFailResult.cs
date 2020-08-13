using System;
using System.Collections.Generic;
using MariGlobals.Extensions;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when one or more param preconditions fails.
    /// </summary>
    public class ParamPreconditionsFailResult : IResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ParamPreconditionsFailResult" /> with the 
        /// specified failed preconditions results.
        /// </summary>
        /// <param name="param">The param that these preconditions failed.</param>
        /// <param name="value">The param value that these preconditions failed.</param>
        /// <param name="failedPreconditions">All failed preconditions results.</param>
        public ParamPreconditionsFailResult(IParameter param, object value, IReadOnlyCollection<(ParamPreconditionAttribute Precondition, IPreconditionResult Result)> failedPreconditions)
        {
            Parameter = param;
            Value = value;
            FailedPreconditions = failedPreconditions;
            Reason = GetReason();
        }

        private string GetReason()
        {
            var oneManyText = FailedPreconditions.Count > 1
                ? "Many preconditions"
                : "One precondition";

            return $"{oneManyText} failed for the param {Parameter.Name} in command {Parameter.Command.Name}";
        }

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason { get; }

        /// <inheritdoc />
        public Exception Exception => null;

        /// <summary>
        /// All failed preconditions.
        /// </summary>
        public IReadOnlyCollection<(ParamPreconditionAttribute Precondition, IPreconditionResult Result)> FailedPreconditions { get; }

        /// <summary>
        /// The parameter that failed.
        /// </summary>
        public IParameter Parameter { get; }

        /// <summary>
        /// The value of that <see cref="Parameter" />.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ParamPreconditionsFailResult" /> with the 
        /// specified failed preconditions results.
        /// </summary>
        /// <param name="param">The param that these preconditions failed.</param>
        /// <param name="value">The param value that these preconditions failed.</param>
        /// <param name="fails">All failed preconditions results.</param>
        /// <returns>A <see cref="ParamPreconditionsFailResult" />.</returns>
        public static ParamPreconditionsFailResult FromFaileds(IParameter param, object value, IReadOnlyCollection<(ParamPreconditionAttribute, IPreconditionResult)> fails)
            => new ParamPreconditionsFailResult(param, value, fails);
    }
}