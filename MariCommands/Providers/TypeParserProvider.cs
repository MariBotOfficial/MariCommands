using System;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Providers
{
    internal sealed class TypeParserProvider : ITypeParserProvider
    {
        private readonly IServiceProvider _provider;

        public TypeParserProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ITypeParser GetTypeParser(Type type)
        {
            var typeParsers = _provider.GetServices<ITypeParser>();

            foreach (var typeParser in typeParsers)
            {
                if (typeParser.CanParse(type))
                    return typeParser;
            }

            return null;
        }
    }
}