namespace MariCommands
{
    /// <inheritdoc />
    public class TypeParserSuccessResult<T> : TypeParserResult<T>
    {
        /// <inheritdoc />
        public TypeParserSuccessResult(T value) : base(value)
        {
        }

        /// <summary>
        /// Build a success type parser result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>A success type parser result.</returns>
        public static TypeParserSuccessResult<T> FromSuccess(T value)
            => new TypeParserSuccessResult<T>(value);
    }
}