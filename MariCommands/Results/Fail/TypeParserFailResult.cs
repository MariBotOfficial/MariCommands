using System;

namespace MariCommands
{
    /// <inheritdoc />
    public class TypeParserFailResult : TypeParserResult
    {
        /// <inheritdoc />
        public TypeParserFailResult(Exception exception) : base(exception)
        {
        }

        /// <inheritdoc />
        public TypeParserFailResult(string reason) : base(reason)
        {
        }

        /// <summary>
        /// Build a failed type parser result.
        /// </summary>
        /// <param name="reason">The reason of the error.</param>
        /// <returns>A failed type parser result.</returns>
        static TypeParserFailResult FromError(string reason)
            => new TypeParserFailResult(reason);

        /// <summary>
        /// Build a faile type parser result.
        /// </summary>
        /// <param name="exception">The exception ocurried during the parsing.</param>
        /// <returns>A failed type parser result.</returns>
        static TypeParserFailResult FromException(Exception exception)
            => new TypeParserFailResult(exception);
    }
}