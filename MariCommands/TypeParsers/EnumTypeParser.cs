using System;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Models;
using MariCommands.Results;
using MariCommands.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.TypeParsers
{
    internal sealed class EnumTypeParser : ITypeParser<Enum>
    {
        private readonly ICommandServiceOptions _config;

        public EnumTypeParser(ICommandServiceOptions config)
        {
            _config = config;
        }

        public bool CanParse(Type type)
        {
            if (!ParsingUtils.IsNullable(type))
                return typeof(Enum).IsAssignableFrom(type);

            if (!_config.TypeParserOfClassIsNullables)
                return false;

            var enumType = type.GetGenericArguments().FirstOrDefault();

            return typeof(Enum).IsAssignableFrom(enumType);
        }

        public bool CanParseInheritedTypes()
            => true;

        public Task<ITypeParserResult<Enum>> ParseAsync(string value, IParameter parameter, CommandContext context)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ITypeParserResult<Enum> result = new TypeParserFailResult<Enum>();

                return Task.FromResult(result);
            }

            var config = context.CommandServices.GetRequiredService<ICommandServiceOptions>();

            var ignoreCase = IsIgnoreCase(config.Comparison);

            if (Enum.TryParse(parameter.ParameterInfo.ParameterType, value, ignoreCase, out var parseResult))
            {
                ITypeParserResult<Enum> result = TypeParserSuccessResult<Enum>.FromValue(parseResult as Enum);

                return Task.FromResult(result);
            }
            else
            {
                ITypeParserResult<Enum> result = new TypeParserFailResult<Enum>();

                return Task.FromResult(result);
            }
        }

        private bool IsIgnoreCase(StringComparison comparison)
        {
            return comparison switch
            {
                StringComparison.Ordinal => true,
                StringComparison.CurrentCulture => true,
                StringComparison.InvariantCulture => true,

                StringComparison.OrdinalIgnoreCase => false,
                StringComparison.CurrentCultureIgnoreCase => false,
                StringComparison.InvariantCultureIgnoreCase => false,

                _ => throw new ArgumentOutOfRangeException(nameof(comparison)),
            };
        }
    }
}