using System;

namespace MariCommands
{
    /// <summary>
    /// Defines the <see cref="RunMode" /> for this module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RunModeAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="RunMode" /> for this module or command.
        /// </summary>
        public RunMode Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="RunModeAttribute" />.
        /// </summary>
        /// <param name="runMode">The <see cref="RunMode" /> for this module or command.</param>
        public RunModeAttribute(RunMode runMode)
        {
            Value = runMode;
        }
    }
}