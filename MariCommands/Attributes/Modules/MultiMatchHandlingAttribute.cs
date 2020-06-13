using System;

namespace MariCommands
{
    /// <summary>
    /// Defines how multi matches will be handled for this module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MultiMatchHandlingAttribute : Attribute
    {
        /// <summary>
        /// How multi matches will be handled for this module.
        /// </summary>
        public MultiMatchHandling Value { get; }


        /// <summary>
        /// Creates a new instance off <see cref="MultiMatchHandlingAttribute" />.
        /// </summary>
        /// <param name="multiMatchHandling">How multi matches will be handled for this module.</param>
        public MultiMatchHandlingAttribute(MultiMatchHandling multiMatchHandling)
        {
            Value = multiMatchHandling;
        }
    }
}