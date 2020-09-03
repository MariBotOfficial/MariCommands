using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Parsers;
using MariCommands.Providers;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.ArgumentParsers
{
    public class ArgumentParserTests
    {
        public ArgumentParserTests()
        {
        }

        private (CommandContext, IArgumentParser) GetContextAndArgumentParser(Action<MariCommandsOptions> options = null)
        {
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();

            if (options.HasContent())
                services.Configure<MariCommandsOptions>(options);

            services.AddTransient<ITypeParserProvider, TypeParserProvider>();

            services.AddAllDefaultTypeParsers(true);

            services.AddTransient<IArgumentParser, ArgumentParser>();

            var provider = services.BuildServiceProvider(true);

            var context = new CommandContext();

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            return (context, context.CommandServices.GetRequiredService<IArgumentParser>());
        }

        [Fact]
        public async Task SuccessIfNoParameters()
        {
            var input = string.Empty;

            var (context, parser) = GetContextAndArgumentParser();

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(new List<IParameter>());

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Empty(result.Args);
        }

        [Fact]
        public async Task MissingTypeParserIfCantFindTypeParser()
        {
            var input = "testInput";

            var (context, parser) = GetContextAndArgumentParser();

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(IParameter));

            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.IsType<MissingTypeParserResult>(result);
            Assert.Equal(param1, (result as MissingTypeParserResult).Parameter);
        }

        [InlineData("value1")]
        [InlineData("value1 value2")]
        [InlineData("value1 value2 value3")]
        [InlineData("value1 value2 value3 value4")]
        [Theory]
        public async Task ParseWithoutRemainderWillIgnoreExtraArgs(string input)
        {
            var (context, parser) = GetContextAndArgumentParser();

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(string));

            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(parameters.Count, result.Args.Count);
        }

        [InlineData("value1")]
        [InlineData("value1 value2")]
        [InlineData("value1 value2 value3")]
        [InlineData("value1 value2 value3 value4")]
        [Theory]
        public async Task ParseWithParamsArgWillParseAllRemainingInput(string input)
        {
            var (context, parser) = GetContextAndArgumentParser();

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(string[]));

            paramMock1
                    .SetupGet(a => a.IsParams)
                    .Returns(true);


            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(parameters.Count, result.Args.Count);
        }


        [Fact]
        public async Task CanParseOptionalParameters()
        {
            string input = null;

            var (context, parser) = GetContextAndArgumentParser();

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(string));

            paramMock1
                    .SetupGet(a => a.IsOptional)
                    .Returns(true);

            paramMock1
                    .SetupGet(a => a.DefaultValue)
                    .Returns("default");


            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(parameters.Count, result.Args.Count);
        }

        [Fact]
        public async Task CanParseNullableClassParameterIfConfigIsEnabled()
        {
            string input = null;

            var (context, parser) = GetContextAndArgumentParser((config) =>
            {
                config.TypeParserOfClassIsNullables = true;
            });

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(string));

            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(parameters.Count, result.Args.Count);
        }

        [Fact]
        public async Task CantParseNullableClassParameterIfConfigIsDisabled()
        {
            string input = null;

            var (context, parser) = GetContextAndArgumentParser((config) =>
            {
                config.TypeParserOfClassIsNullables = false;
            });

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(string));

            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        [Fact]
        public async Task CanParseNullableParameterIfHasTypeParser()
        {
            string input = null;

            var (context, parser) = GetContextAndArgumentParser((config) =>
            {
                config.TypeParserOfClassIsNullables = false;
            });

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(int?));

            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(parameters.Count, result.Args.Count);
        }

        [InlineData("value1")]
        [InlineData("value1 value2")]
        [InlineData("value1 value2 value3")]
        [InlineData("value1 value2 value3 value4")]
        [Theory]
        public async Task ParseWithRemainderArgWillParseAllRemainingInput(string input)
        {
            var (context, parser) = GetContextAndArgumentParser();

            var paramMock1 = new Mock<IParameter>();

            paramMock1
                    .SetupGet(a => a.ParameterInfo.ParameterType)
                    .Returns(typeof(string));

            paramMock1
                    .SetupGet(a => a.IsRemainder)
                    .Returns(true);

            paramMock1
                    .SetupGet(a => a.Name)
                    .Returns("param");

            var param1 = paramMock1.Object;

            var parameters = new List<IParameter>
            {
                param1,
            };

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(parameters);

            var command = commandMock.Object;

            var result = await parser.ParseAsync(context, command, input);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(parameters.Count, result.Args.Count);
        }
    }
}