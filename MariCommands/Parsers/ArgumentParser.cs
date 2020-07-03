using System;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <inheritdoc />
    public class ArgumentParser : IArgumentParser
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Creates a new instance of <see cref="ArgumentParser" />.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public ArgumentParser(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
        }

        /// <inheritdoc />
        public Task<IArgumentParserResult> ParseAsync(CommandContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}