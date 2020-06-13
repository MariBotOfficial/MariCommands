using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands
{
    public class CommandFactory : ICommandFactory
    {

        /// <summary>
        ///     Create a news instace of <see  cref="CommandFactory"/>
        /// </summary>
        /// <param name="provider">An <see cref="IServiceProvider" />.</param>

        private readonly IServiceProvider _provider;


        public CommandFactory(IServiceProvider provider)
        {
            _provider = provider;
        }


        /// <inheritdoc>
        public async Task<ICommandBuilder> BuildCommandAsync(Type type, MethodInfo methodInfo)
        {
            if (await IsCommandAsync(type, methodInfo))
                throw new Exception($"man this command({methodInfo}) is not valid, please pay attention");
            
            return null;
        }

        /// <inheritdoc>
        public Task<bool> IsCommandAsync(Type type, MethodInfo methodInfo)
        {
            var isValid = type.HasContent() &&
                          methodInfo.HasContent() &&
                          methodInfo.CustomAttributes.Any(a => a.AttributeType.IsEquivalentTo(typeof(CommandAttribute)));

            return Task.FromResult(isValid);
        }
    }
}