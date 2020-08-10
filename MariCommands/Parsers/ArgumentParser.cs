using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

                var typeParser = GetTypeParser(context, param);
            }


            return Task.FromResult<IArgumentParserResult>(null);
        }

        private ITypeParser GetTypeParser(CommandContext context, IParameter param)
        {
            throw new NotImplementedException();
        }

        private bool IsLastParam(int position, IEnumerable<string> rawArgs)
            => rawArgs.Count() - 1 == position;
    }
}