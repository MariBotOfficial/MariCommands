using System;

namespace MariCommands
{
    /// <summary>
    /// Define any remarks for this module, command or param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class RemarksAttribute : Attribute
    {
        /// <summary>
        /// Any remarks for this module, command or param.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Creates a new instance off <see cref="RemarksAttribute" />.
        /// </summary>
        /// <param name="remarks">Any remarks for this module, command or param.</param>
        public RemarksAttribute(string remarks)
        {
            remarks.NotNullOrWhiteSpace(nameof(remarks));

            Value = remarks;
        }
    }
}