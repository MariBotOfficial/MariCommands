using System;

namespace MariCommands
{
    /// <summary>
    /// Defines the <see cref="ModuleLifetime" /> of this module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LifetimeAttribute : Attribute
    {
        /// <summary>
        /// The <see cref="ModuleLifetime" /> of this module.
        /// </summary>
        public ModuleLifetime Value { get; }

        /// <summary>
        /// Creates a new instance of <see cref="LifetimeAttribute" />.
        /// </summary>
        /// <param name="lifeTime">The <see cref="ModuleLifetime" /> of this module.</param>
        public LifetimeAttribute(ModuleLifetime lifeTime)
        {
            Value = lifeTime;
        }
    }
}