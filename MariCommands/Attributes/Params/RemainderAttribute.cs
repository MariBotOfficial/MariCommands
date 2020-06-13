using System;

namespace MariCommands
{
    /// <summary>
    /// Defines all remaining text will be used for parse this param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RemainderAttribute : Attribute
    {

    }
}