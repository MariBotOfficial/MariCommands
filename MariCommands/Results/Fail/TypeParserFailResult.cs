using System;

namespace MariCommands.Results
{
    /// <summary>
    /// Represents when a type parser fails to parse a result.
    /// </summary>
    public class TypeParserFailResult<T> : ITypeParserResult<T>
    {
        /// <inheritdoc />
        public T Value => default;

        /// <inheritdoc />
        public bool Success => false;

        /// <inheritdoc />
        public string Reason => $"Cannot parse {typeof(T).Name}.";

        /// <inheritdoc />
        public Exception Exception => null;
    }
}