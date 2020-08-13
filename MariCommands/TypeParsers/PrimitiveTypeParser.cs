using System.Threading.Tasks;
using MariCommands.Models;
using MariCommands.Results;
using MariCommands.Utils;

namespace MariCommands.TypeParsers
{
    internal sealed class PrimitiveTypeParser<T> : ITypeParser<T>
        where T : struct
    {
        private static readonly TryParseDelegate<T> _tryParseDelegate;

        static PrimitiveTypeParser()
        {
            _tryParseDelegate = ParsingUtils.GetParseDelegate<T>();
        }

        public bool CanParseInheritedTypes()
            => false;

        public Task<ITypeParserResult<T>> ParseAsync(string value, IParameter parameter, CommandContext context)
        {
            if (_tryParseDelegate(value, out var parseResult))
            {
                ITypeParserResult<T> result = TypeParserSuccessResult<T>.FromValue(parseResult);

                return Task.FromResult(result);
            }
            else
            {
                ITypeParserResult<T> result = new TypeParserFailResult<T>();

                return Task.FromResult(result);
            }
        }
    }
}