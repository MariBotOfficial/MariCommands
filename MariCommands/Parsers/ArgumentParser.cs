using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Providers;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Parsers
{
    /// <inheritdoc />
    internal sealed class ArgumentParser : IArgumentParser
    {
        public ArgumentParser()
        {
        }

        public Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommandMatch match)
        {
            var provider = context.CommandServices;

            var config = provider.GetRequiredService<ICommandServiceOptions>();

            var rawArgs = match.RemainingInput.Split(config.Separator);

            var willFaultParams = rawArgs.Length < match.Command.Parameters.Count;

            for (var i = 0; i < rawArgs.Length; i++)
            {
                var arg = rawArgs[i];
                var param = match.Command.Parameters.ElementAt(i);

                var typeParser = GetTypeParser(provider, param);

                if (typeParser.HasContent())
                    return Task.FromResult<IArgumentParserResult>(MissingTypeParserResult.FromParam(param));
            }


            return Task.FromResult<IArgumentParserResult>(null);
        }

        private ITypeParser GetTypeParser(IServiceProvider provider, IParameter param)
        {
            var typeParserType = param.TypeParserType;

            if (typeParserType.HasContent())
                return ActivatorUtilities.GetServiceOrCreateInstance(provider, typeParserType) as ITypeParser;

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            return typeParserProvider.GetTypeParser(param.ParameterInfo.ParameterType);
        }

        private bool IsLastParam(int position, IEnumerable<string> rawArgs)
            => rawArgs.Count() - 1 == position;
    }
}