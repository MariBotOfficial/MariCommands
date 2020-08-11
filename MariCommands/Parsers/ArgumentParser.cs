using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Providers;
using MariCommands.Results;
using MariCommands.TypeParsers;
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

        public async Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommandMatch match)
        {
            var provider = context.CommandServices;

            var config = provider.GetRequiredService<ICommandServiceOptions>();

            var rawArgs = match.RemainingInput.Split(config.Separator);

            var willFaultParams = rawArgs.Length < match.Command.Parameters.Count;

            var args = new Dictionary<IParameter, object>();

            for (var i = 0; i < rawArgs.Length; i++)
            {
                var arg = rawArgs[i];
                var param = match.Command.Parameters.ElementAt(i);
                var typeParser = GetTypeParser(provider, param);

                if (typeParser.HasContent())
                    return MissingTypeParserResult.FromParam(param);

                var isLastParam = i + 1 == match.Command.Parameters.Count;

                if (isLastParam && param.IsParams)
                {
                    var multipleArgs = args.Skip(i).ToList();

                    var multipleValues = new List<object>();

                    foreach (var multipleArg in multipleArgs)
                    {
                        var result = await typeParser.ParseAsync(arg, param, context);

                        if (!result.Success)
                            return ArgumentTypeParserFailResult.FromTypeParserResult(result);

                        multipleValues.Add(result.Value);
                    }

                    args.Add(param, multipleValues);
                }
                else
                {
                    if (isLastParam && param.IsRemainder)
                    {
                        arg = string.Join(config.Separator, args.Skip(i).ToList());
                    }

                    var result = await typeParser.ParseAsync(arg, param, context);

                    if (!result.Success)
                        return ArgumentTypeParserFailResult.FromTypeParserResult(result);

                    args.Add(param, result.Value);
                }
            }

            if (willFaultParams)
            {
                var missingParams = GetMissingParams(rawArgs.Length, match.Command.Parameters);

                foreach (var param in missingParams)
                {
                    if (param.IsOptional)
                    {
                        args.Add(param, param.DefaultValue);
                    }
                    else if (IsNullable(param) || IsNullableClass(param, config))
                    {
                        var typeParser = GetTypeParser(provider, param);

                        if (typeParser.HasContent())
                            return MissingTypeParserResult.FromParam(param);

                        var result = await typeParser.ParseAsync(null, param, context);

                        if (!result.Success)
                            return ArgumentTypeParserFailResult.FromTypeParserResult(result);

                        args.Add(param, null);
                    }
                    else
                    {
                        return BadArgCountParseResult.FromCommand(match.Command);
                    }
                }
            }

            return ArgumentParseSuccessResult.FromArgs(args);
        }

        private bool IsNullable(IParameter param)
            =>
                param.ParameterInfo.ParameterType.IsGenericType &&
                param.ParameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

        private bool IsNullableClass(IParameter param, ICommandServiceOptions config)
        {
            if (!param.ParameterInfo.ParameterType.IsClass)
                return false;

            return config.TypeParserOfClassIsNullables;
        }

        private IEnumerable<IParameter> GetMissingParams(int length, IReadOnlyCollection<IParameter> parameters)
        {
            return parameters
                        .Skip(length)
                        .ToList();
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