using System;

namespace MariCommands
{
    /// <summary>
    /// Defines a precondition for this command or module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PreconditionAttribute : Attribute
    {
        // TODO: ExecuteAsync
    }
}