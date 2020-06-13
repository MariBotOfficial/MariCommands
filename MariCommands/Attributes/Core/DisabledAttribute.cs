using System;

namespace MariCommands
{
    /// <summary>
    /// Disable this module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisabledAttribute : Attribute
    {

    }
}