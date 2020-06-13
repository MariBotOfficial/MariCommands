using System;

namespace MariCommands.Attributes.Params
{
    /// <summary>
    /// Defines all remaining text will be used for parse this param.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RemainderAttribute : Attribute
    {

    }
}