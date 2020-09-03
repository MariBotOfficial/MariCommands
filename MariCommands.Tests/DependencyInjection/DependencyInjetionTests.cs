using System;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Parsers;
using MariCommands.Providers;
using MariCommands.Results;
using MariCommands.TypeParsers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.DependencyInjection
{
    public class DependencyInjetionTests
    {
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [Theory]
        public void NotThrowExceptionForAnyOption(bool addAllDefaultTypeParsers, bool createNullables)
        {
            var services = new ServiceCollection();

            services.AddBasicMariCommandsServices((config) =>
            {
                config.AddAllDefaultTypeParsers = addAllDefaultTypeParsers;
                config.CreateNullables = createNullables;
            });

            var provider = services.BuildServiceProvider(true);
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public void NotThrowExceptionForAnyOptionTryParser(bool createNullables)
        {
            var services = new ServiceCollection();

            services.AddAllDefaultTypeParsers(createNullables);

            var provider = services.BuildServiceProvider(true);
        }

        [InlineData(typeof(char))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        [Theory]
        public void CanRetrieveAllPrimitiveTypeParsers(Type type)
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddPrimitiveTypeParsers(false);

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(type);

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom(typeof(ITypeParser<>).MakeGenericType(type), typeParser);
            Assert.IsType(typeof(PrimitiveTypeParser<>).MakeGenericType(type), typeParser);
        }

        [InlineData(typeof(char?))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(byte?))]
        [InlineData(typeof(sbyte?))]
        [InlineData(typeof(short?))]
        [InlineData(typeof(ushort?))]
        [InlineData(typeof(int?))]
        [InlineData(typeof(uint?))]
        [InlineData(typeof(long?))]
        [InlineData(typeof(ulong?))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(decimal?))]
        [Theory]
        public void CanRetrieveAllNullablePrimitiveTypeParsers(Type type)
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddPrimitiveTypeParsers(true);

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(type);

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom(typeof(ITypeParser<>).MakeGenericType(type), typeParser);
            Assert.IsType(typeof(NullableTypeParser<>).MakeGenericType(type.GetGenericArguments()[0]), typeParser);
        }

        [InlineData(typeof(char?))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(byte?))]
        [InlineData(typeof(sbyte?))]
        [InlineData(typeof(short?))]
        [InlineData(typeof(ushort?))]
        [InlineData(typeof(int?))]
        [InlineData(typeof(uint?))]
        [InlineData(typeof(long?))]
        [InlineData(typeof(ulong?))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(decimal?))]
        [Theory]
        public void CantRetrieveAllNullablePrimitiveTypeParsers(Type type)
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddPrimitiveTypeParsers(false);

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(type);

            Assert.Null(typeParser);
        }

        [Fact]
        public void CanRetriveStringTypeParser()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddStringTypeParser();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(string));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<string>>(typeParser);
            Assert.IsType<StringTypeParser>(typeParser);
        }

        [Fact]
        public void CanRetriveEnumTypeParser()
        {
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();
            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddEnumTypeParser();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(Enum));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<Enum>>(typeParser);
            Assert.IsType<EnumTypeParser>(typeParser);
        }

        [Fact]
        public void CanRetriveEnumTypeParserFromAnotherEnum()
        {
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();
            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddEnumTypeParser();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(SampleEnum));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<Enum>>(typeParser);
            Assert.IsType<EnumTypeParser>(typeParser);
        }

        [InlineData(false)]
        [InlineData(true)]
        [Theory]
        public void CanRetriveEnumTypeParserFromAnotherNullableEnumIfTypeClassIsEnabled(bool isEnabled)
        {
            var services = new ServiceCollection();

            services.Configure<MariCommandsOptions>(config =>
            {
                config.TypeParserOfClassIsNullables = isEnabled;
            });

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddEnumTypeParser();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(SampleEnum?));

            Assert.Equal(isEnabled, typeParser != null);

            if (isEnabled)
            {
                Assert.IsAssignableFrom<ITypeParser<Enum>>(typeParser);
                Assert.IsType<EnumTypeParser>(typeParser);
            }
        }

        [Fact]
        public void CanOverrideTypeParser()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddStringTypeParser();
            services.AddTypeParser<TestTypeParser<string>, string>();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(string));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<string>>(typeParser);
            Assert.IsType<TestTypeParser<string>>(typeParser);
        }

        [Fact]
        public void CanOverrideDependency()
        {
            var services = new ServiceCollection();

            services.AddBasicMariCommandsServices();
            services.AddTransient<IArgumentParser, TestArgumentParser>();

            var provider = services.BuildServiceProvider(true);

            var argumentParser = provider.GetRequiredService<IArgumentParser>();

            Assert.NotNull(argumentParser);
            Assert.IsAssignableFrom<IArgumentParser>(argumentParser);
            Assert.IsType<TestArgumentParser>(argumentParser);
        }

        [Fact]
        public void CanAddTypeClassParser()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddTypeParser<TestTypeParser<string>, string>();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(string));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<string>>(typeParser);
            Assert.IsType<TestTypeParser<string>>(typeParser);
        }

        [Fact]
        public void CanAddTypeStructParser()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddTypeParser<TestTypeParser<int>, int>(false);

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(int));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<int>>(typeParser);
            Assert.IsType<TestTypeParser<int>>(typeParser);
        }

        [Fact]
        public void CanAddNullableTypeStructParser()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddTypeParser<TestTypeParser<int?>, int?>();

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(int?));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<int?>>(typeParser);
            Assert.IsType<TestTypeParser<int?>>(typeParser);
        }

        [Fact]
        public void CanAddNullableTypeStructParserFromOption()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddTypeParser<TestTypeParser<int>, int>(true);

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(int?));

            Assert.NotNull(typeParser);
            Assert.IsAssignableFrom<ITypeParser<int?>>(typeParser);
            Assert.IsType<NullableTypeParser<int>>(typeParser);
        }

        [Fact]
        public void NotRetrieveNullableTypeParserWhenOptionFalse()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddTypeParser<TestTypeParser<int>, int>(false);

            var provider = services.BuildServiceProvider(true);

            var typeParserProvider = provider.GetRequiredService<ITypeParserProvider>();

            var typeParser = typeParserProvider.GetTypeParser(typeof(int?));

            Assert.Null(typeParser);
        }
    }

    public enum SampleEnum
    {
        Value1,
        Value2,
    }

    public class TestTypeParser<T> : ITypeParser<T>
    {
        public bool CanParseInheritedTypes()
            => false;

        public Task<ITypeParserResult<T>> ParseAsync(string value, IParameter parameter, CommandContext context)
            => throw new NotSupportedException();
    }

    public class TestArgumentParser : IArgumentParser
    {
        public Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommand command, string rawArgs)
            => throw new NotSupportedException();
    }
}