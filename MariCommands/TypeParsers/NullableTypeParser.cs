using System;
using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.TypeParsers
{
    internal sealed class NullableTypeParser<T> : ITypeParser<T?>
        where T : struct
    {
        private readonly ITypeParser<T> _nonNullableTypeParser;

        public NullableTypeParser(ITypeParser<T> nonNullableTypeParser)
        {
            _nonNullableTypeParser = nonNullableTypeParser;
        }

        public bool CanParseInheritedTypes()
            => _nonNullableTypeParser.CanParseInheritedTypes();

        public async Task<ITypeParserResult<T?>> ParseAsync(string value, IParameter parameter, CommandContext context)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new NullableTypeParserResult<T>();
            }
            else
            {
                var result = await _nonNullableTypeParser.ParseAsync(value, parameter, context);

                return NullableTypeParserResult<T>.FromResult(result);
            }
        }
    }
}