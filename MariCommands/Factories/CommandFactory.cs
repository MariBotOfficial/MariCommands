using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _provider;
        private readonly ICommandServiceOptions _config;

        /// <summary>
        /// Create a new instace of <see  cref="CommandFactory"/>.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public CommandFactory(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.Instance;
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
        }


        /// <inheritdoc />
        public async Task<ICommandBuilder> BuildCommandAsync(Type type, MethodInfo methodInfo)
        {
            type.NotNull(nameof(type));
            methodInfo.NotNull(nameof(methodInfo));

            if (!await IsCommandAsync(type, methodInfo))
                throw new ArgumentException(nameof(type), $"{methodInfo.Name} is not a valid command.");

            return null;
        }

        /// <inheritdoc />
        public Task<bool> IsCommandAsync(Type type, MethodInfo methodInfo)
        {
            var isValid = type.HasContent() &&
                          methodInfo.HasContent() &&
                          methodInfo.CustomAttributes.Any(a => a.AttributeType.IsEquivalentTo(typeof(CommandAttribute)));

            return Task.FromResult(isValid);
        }
    }
}