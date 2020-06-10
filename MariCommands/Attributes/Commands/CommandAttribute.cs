using System;

namespace MariCommands
{
    /// <summary>
    /// Mark a method to be a Command.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {

    }
}