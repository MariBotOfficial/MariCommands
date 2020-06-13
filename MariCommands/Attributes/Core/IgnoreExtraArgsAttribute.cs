using System;

namespace MariCommands
{
    /// <summary>
    /// Defines if this module or command will ignore extra args.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IgnoreExtraArgsAttribute : Attribute
    {
        /// <summary>
        /// If this module or command will ignore extra args.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Creates a new instance off <see cref="IgnoreExtraArgsAttribute" />
        /// with value: <see langword="true" />.
        /// </summary>
        public IgnoreExtraArgsAttribute() : this(true)
        {

        }

        /// <summary>
        /// Creates a new instance off <see cref="IgnoreExtraArgsAttribute" />.
        /// </summary>
        /// <param name="ignoreExtraArgs">If this module or command will ignore extra args.</param>
        public IgnoreExtraArgsAttribute(bool ignoreExtraArgs)
        {
            Value = ignoreExtraArgs;
        }
    }
}