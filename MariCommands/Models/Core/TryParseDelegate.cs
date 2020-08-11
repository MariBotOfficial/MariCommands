using System;

namespace MariCommands.Models
{
    internal delegate bool TryParseDelegate<T>(ReadOnlySpan<char> value, out T result)
        where T : struct;
}