using System;

namespace MariCommands
{
    /// <summary>
    /// Prevents this specific module (not for inherited modules) to be listed in Modules.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DontLoadAttribute : Attribute
    {

    }
}