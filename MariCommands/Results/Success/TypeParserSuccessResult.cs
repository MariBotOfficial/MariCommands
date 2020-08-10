namespace MariCommands
{
    /// <inheritdoc />
    public class TypeParserSuccessResult : TypeParserResult
    {
        /// <inheritdoc />
        public TypeParserSuccessResult(object value) : base(value)
        {
        }

        /// <summary>
        /// Build a success type parser result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A success type parser result.</returns>
        public static TypeParserSuccessResult FromSuccess(object value)
            => new TypeParserSuccessResult(value);
    }
}