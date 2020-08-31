using MariCommands.Extensions;
using MariCommands.Providers;
using MariCommands.TypeParsers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Providers
{
    public class TypeParserProviderTests
    {
        [Fact]
        public void CanGetFromTypeParsers()
        {
            var services = new ServiceCollection();

            services.AddStringTypeParser();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(string));

            Assert.NotNull(typeParser);
            Assert.IsType<StringTypeParser>(typeParser);
        }

        [Fact]
        public void NullIfCannotGetTypeParser()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(string));

            Assert.Null(typeParser);
        }
    }
}