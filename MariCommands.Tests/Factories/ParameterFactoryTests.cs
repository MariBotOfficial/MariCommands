using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Factories;
using MariCommands.Results;
using MariCommands.TypeParsers;
using Moq;
using Xunit;

namespace MariCommands.Tests.Factories
{
    public class ParameterFactoryTests
    {

        [Fact]
        public void IsInvalidParameterIfEverythingIsNotNull()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;
            var parameter = new Mock<ParameterInfo>().Object;

            var result = factory.IsParameter(commandBuilder, parameter);

            Assert.True(result);
        }

        [Fact]
        public void IsInvalidParameterIfCommandBuilderIsNull()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var parameter = new Mock<ParameterInfo>().Object;

            var result = factory.IsParameter(null, parameter);

            Assert.False(result);
        }

        [Fact]
        public void IsInvalidParameterIfParameterInfoIsNull()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var result = factory.IsParameter(commandBuilder, null);

            Assert.False(result);
        }

        [Fact]
        public void IsInvalidParameterIfEverythingIsNull()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var result = factory.IsParameter(null, null);

            Assert.False(result);
        }

        [Fact]
        public void ThrowExceptionIfInvalidAndTryBuild()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            Assert.ThrowsAny<ArgumentException>(() =>
            {
                _ = factory.BuildParameter(null, null);
            });
        }

        [Fact]
        public void CanGetParameterNameFromAttribute()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Name));
            Assert.Equal(TestParameterClass.ParameterName, builder.Name);
        }

        [Fact]
        public void CanGetParameterName()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethod))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Name));
            Assert.Equal(TestParameterClass.ParameterName, builder.Name);
        }

        [Fact]
        public void CanGetDescriptionIfExists()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Description));
            Assert.Equal(TestParameterClass.ParameterDescription, builder.Description);
        }

        [Fact]
        public void CantGetDescriptionIfNotExists()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethod))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.True(string.IsNullOrWhiteSpace(builder.Description));
        }

        [Fact]
        public void CanGetRemarksIfExists()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Remarks));
            Assert.Equal(TestParameterClass.ParameterRemarks, builder.Remarks);
        }

        [Fact]
        public void CantGetRemarksIfNotExists()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethod))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.True(string.IsNullOrWhiteSpace(builder.Remarks));
        }

        [Fact]
        public void CanGetIfParams()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodParams))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.True(builder.IsParams);
        }

        [Fact]
        public void CantGetIfNotParams()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethod))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.False(builder.IsParams);
        }

        [Fact]
        public void CanGetOptionalAndDefaultValue()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodOptional))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.True(builder.IsOptional);
            Assert.NotNull(builder.DefaultValue);
            Assert.Equal(TestParameterClass.ParameterDefaultValue, builder.DefaultValue);
        }

        [Fact]
        public void CantGetOptionalAndDefaultValue()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethod))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.False(builder.IsOptional);
            Assert.Null(builder.DefaultValue);
        }

        [Fact]
        public void CanGetTypeParserType()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.NotNull(builder.TypeParserType);
            Assert.Equal(typeof(TestParameterClass), builder.TypeParserType);
        }

        [Fact]
        public void CantGetTypeParserType()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethod))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.Null(builder.TypeParserType);
        }

        [Fact]
        public void CanGetAllAttributes()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var attributes = parameter.GetCustomAttributes();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Attributes);
            Assert.Equal(attributes, builder.Attributes);
        }

        [Fact]
        public void CanGetAllPreconditions()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameter = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var preconditions = parameter
                                .GetCustomAttributes()
                                .Where(a => typeof(ParamPreconditionAttribute).IsAssignableFrom(a.GetType()))
                                .Select(a => a as ParamPreconditionAttribute)
                                .ToList();

            var builder = factory.BuildParameter(commandBuilder, parameter);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Preconditions);
            Assert.Equal(preconditions, builder.Preconditions);
        }

        [Fact]
        public void CanBuild()
        {
            var options = new MariCommandsOptions();
            var factory = new ParameterFactory(options);

            var commandBuilder = new Mock<ICommandBuilder>().Object;

            var parameterInfo = typeof(TestParameterClass)
                            .GetMethod(nameof(TestParameterClass.TestMethodAttribute))
                            .GetParameters()
                            .FirstOrDefault();

            var builder = factory.BuildParameter(commandBuilder, parameterInfo);

            var command = new Mock<ICommand>().Object;

            var parameter = builder.Build(command);

            Assert.NotNull(parameter);
        }
    }

    public class TestParameterClass : ITypeParser
    {
        public const string ParameterName = "testName";
        public const string ParameterDescription = "test description";
        public const string ParameterRemarks = "test remarks";
        public const string ParameterDefaultValue = "test value";

        public void TestMethodAttribute(
            [Name(ParameterName)]
            [Description(ParameterDescription)]
            [Remarks(ParameterRemarks)]
            [TypeParser(typeof(TestParameterClass))]
            [TestParamPrecondition]
            string parameter)
        { }

        public void TestMethod(string testName)
        {
            if (nameof(testName) != ParameterName)
                throw new InvalidOperationException($"The {nameof(ParameterName)} value changed.");
        }

        public void TestMethodParams(params string[] parameter)
        { }

        public void TestMethodOptional(string parameter = ParameterDefaultValue)
        { }

        public bool CanParse(Type type)
             => false;

        public Task<ITypeParserResult> ParseAsync(string value, IParameter parameter, CommandContext context)
            => throw new NotSupportedException();
    }

    public class TestParamPreconditionAttribute : ParamPreconditionAttribute
    {
        public override Task<IPreconditionResult> ExecuteAsync(object value, IParameter parameter, CommandContext context)
            => throw new NotSupportedException();
    }
}