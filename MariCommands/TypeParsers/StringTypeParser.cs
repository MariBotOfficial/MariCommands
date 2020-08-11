using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.TypeParsers
{
    internal sealed class StringTypeParser : ITypeParser<string>
    {
        public bool CanParseInheritedTypes()
            => false;

        public Task<ITypeParserResult<string>> ParseAsync(string value, IParameter parameter, CommandContext context)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ITypeParserResult<string> result = new TypeParserFailResult<string>();

                return Task.FromResult(result);
            }
            else
            {
                ITypeParserResult<string> result = TypeParserSuccessResult<string>.FromValue(value);

                return Task.FromResult(result);
            }
        }
    }
}