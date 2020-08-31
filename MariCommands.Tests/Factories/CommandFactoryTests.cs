using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Factories;
using MariCommands.Parsers;
using MariCommands.Providers;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Factories
{
    public class CommandFactoryTests
    {
        private readonly CommandFactory _factory;

        public CommandFactoryTests()
        {
            var options = new CommandServiceOptions();
            var parameterFactory = new ParameterFactory(options);
            var executorProvider = new CommandExecutorProvider();
            var factory = new CommandFactory(options, parameterFactory, executorProvider);

            _factory = factory;
        }

        [Fact]
        public void IsValidCommand()
        {
            var moduleBuilder = new Mock<IModuleBuilder>().Object;

            var method = typeof(TestCommandClass).GetMethod(nameof(TestCommandClass.ValidCommand));

            var result = _factory.IsCommand(moduleBuilder, method);

            Assert.True(result);
        }

        [Fact]
        public void IsInvalidIfMethodIsNull()
        {
            var moduleBuilder = new Mock<IModuleBuilder>().Object;

            var result = _factory.IsCommand(moduleBuilder, null);

            Assert.False(result);
        }

        [Fact]
        public void InvalidIfModuleIsNull()
        {
            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var result = _factory.IsCommand(null, method);

            Assert.False(result);
        }

        [Fact]
        public void InvalidIfNotPublic()
        {
            var moduleBuilder = new Mock<IModuleBuilder>().Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.InternalMethod));

            var result = _factory.IsCommand(moduleBuilder, method);

            Assert.False(result);
        }

        [Fact]
        public void InvalidIfStatic()
        {
            var moduleBuilder = new Mock<IModuleBuilder>().Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.StaticMethod));

            var result = _factory.IsCommand(moduleBuilder, method);

            Assert.False(result);
        }

        [Fact]
        public void InvalidIfGeneric()
        {
            var moduleBuilder = new Mock<IModuleBuilder>().Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.GenericMethod));

            var result = _factory.IsCommand(moduleBuilder, method);

            Assert.False(result);
        }

        [Fact]
        public void InvalidIfNotHasCmdAttribute()
        {
            var moduleBuilder = new Mock<IModuleBuilder>().Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.InvalidCommand));

            var result = _factory.IsCommand(moduleBuilder, method);

            Assert.False(result);
        }

        [Fact]
        public void ThrowsExceptionIfInvalid()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.InvalidCommand));

            Assert.Throws<ArgumentException>(() =>
            {
                _ = _factory.BuildCommand(moduleBuilder, method);
            });
        }

        [Fact]
        public void CanGetAlias()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Aliases);
            Assert.Equal(new string[] { TestCommandClass.CommandName }, builder.Aliases.ToArray());
        }

        [Fact]
        public void CanGetCommandName()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Name));
            Assert.Equal(TestCommandClass.CommandName, builder.Name);
        }

        [Fact]
        public void CanGetDescriptionIfExists()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Description));
            Assert.Equal(TestCommandClass.CommandDescription, builder.Description);
        }

        [Fact]
        public void CantGetDescriptionIfNotExists()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommandWithNoAttributes));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(string.IsNullOrWhiteSpace(builder.Description));
        }

        [Fact]
        public void CanGetRemarksIfExists()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Remarks));
            Assert.Equal(TestCommandClass.CommandRemarks, builder.Remarks);
        }

        [Fact]
        public void CantGetRemarksIfNotExists()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommandWithNoAttributes));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(string.IsNullOrWhiteSpace(builder.Remarks));
        }

        [Fact]
        public void CanGetCustomPriority()
        {
            // Priority is a integer, by default your value will be 0
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.Equal(TestCommandClass.CommandPriority, builder.Priority);
        }

        [Fact]
        public void CanGetCustomRunMode()
        {
            // RunMode is a enum and if we can't retrieve it from the command
            // it will be the configured value, that have default value as "Sequential".

            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.Equal(TestCommandClass.CommandRunMode, builder.RunMode);
        }

        [Fact]
        public void CanGetCustomIgnoreExtraArgs()
        {
            // IgnoreExtraArgs is a boolean and if we can't retrieve it from the command
            // it will be the configuration value, that have default value as "false"

            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.Equal(TestCommandClass.CommandIgnoreExtraArgs, builder.IgnoreExtraArgs);
        }

        [Fact]
        public void CanGetArgumentParserTypeIfExists()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.NotNull(builder.ArgumentParserType);
            Assert.Equal(typeof(TestCommandClass), builder.ArgumentParserType);
        }

        [Fact]
        public void CantGetArgumentParserTypeIfNotExists()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommandWithNoAttributes));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.Null(builder.ArgumentParserType);
        }

        [Fact]
        public void CanGetAllAttributes()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var attributes = method.GetCustomAttributes();

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Attributes);
            Assert.Equal(attributes.Count(), builder.Attributes.Count);
        }

        [Fact]
        public void CanGetAllPreconditions()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var preconditions = method.GetCustomAttributes()
                                .Where(a => typeof(PreconditionAttribute).IsAssignableFrom(a.GetType()))
                                .ToList();

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Preconditions);
            Assert.Equal(preconditions.Count(), builder.Preconditions.Count);
        }

        [Fact]
        public void CanGetDisabled()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.DisabledCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(builder.IsEnabled);
        }

        [Fact]
        public void CanGetEnabled()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsEnabled);
        }

        [Fact]
        public void VoidTaskIsAsync()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.VoidTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
        }

        [Fact]
        public void GenericTaskIsAsync()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.GenericTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
        }

        [Fact]
        public void VoidValueTaskIsAsync()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.VoidValueTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
        }

        [Fact]
        public void GenericValueTaskIsAsync()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.GenericValueTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
        }

        [Fact]
        public void VoidIsNotAsync()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(builder.IsAsync);
        }

        [Fact]
        public void ObjectIsNotAsync()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ObjectCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(builder.IsAsync);
        }

        [Fact]
        public void CanGetAsyncResultTypeFromTask()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.GenericTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
            Assert.NotNull(builder.AsyncResultType);
            Assert.Equal(typeof(object), builder.AsyncResultType);
        }

        [Fact]
        public void CanGetAsyncResultTypeFromValueTask()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.GenericValueTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
            Assert.NotNull(builder.AsyncResultType);
            Assert.Equal(typeof(object), builder.AsyncResultType);
        }

        [Fact]
        public void CantGetAsyncResultTypeFromTask()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.VoidTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
            Assert.Null(builder.AsyncResultType);
        }

        [Fact]
        public void CantGetAsyncResultTypeFromValueTask()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.VoidValueTaskCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.True(builder.IsAsync);
            Assert.Null(builder.AsyncResultType);
        }

        [Fact]
        public void CantGetAsyncResultTypeFromVoid()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(builder.IsAsync);
            Assert.Null(builder.AsyncResultType);
        }

        [Fact]
        public void CantGetAsyncResultTypeFromObject()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ObjectCommand));

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.False(builder.IsAsync);
            Assert.Null(builder.AsyncResultType);
        }

        [Fact]
        public void CanGetParameters()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ParameterCommand));

            var parameters = method.GetParameters();

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Parameters);
            Assert.Equal(parameters.Length, builder.Parameters.Count);
        }

        [Fact]
        public void CantGetParameters()
        {
            var moduleMock = new Mock<IModuleBuilder>();

            moduleMock.SetupGet(a => a.Type).Returns(typeof(TestCommandClass));

            var moduleBuilder = moduleMock.Object;

            var method = typeof(TestCommandClass)
                .GetMethod(nameof(TestCommandClass.ValidCommand));

            var parameters = method.GetParameters();

            var builder = _factory.BuildCommand(moduleBuilder, method);

            Assert.NotNull(builder);
            Assert.Empty(builder.Parameters);
            Assert.Equal(parameters.Length, builder.Parameters.Count);
        }
    }

    public class TestCommandClass : IArgumentParser
    {
        public const string CommandName = "testName";
        public const string CommandDescription = "test description";
        public const string CommandRemarks = "test remarks";
        public const int CommandPriority = 100;
        public const RunMode CommandRunMode = RunMode.Concurrent;
        public const bool CommandIgnoreExtraArgs = true;

        [Command(CommandName)]
        [Description(CommandDescription)]
        [Remarks(CommandRemarks)]
        [Priority(CommandPriority)]
        [RunMode(CommandRunMode)]
        [IgnoreExtraArgs(CommandIgnoreExtraArgs)]
        [ArgumentParser(typeof(TestCommandClass))]
        [TestClassPrecondition]
        public void ValidCommand()
        { }

        [Command(CommandName)]
        public void ValidCommandWithNoAttributes()
        { }

        [Disabled]
        [Command(CommandName)]
        public void DisabledCommand()
        { }

        [Command(CommandName)]
        public Task VoidTaskCommand()
            => Task.CompletedTask;

        [Command(CommandName)]
        public Task<object> GenericTaskCommand()
            => Task.FromResult(new object());

        [Command(CommandName)]
        public ValueTask VoidValueTaskCommand()
            => new ValueTask();

        [Command(CommandName)]
        public ValueTask<object> GenericValueTaskCommand()
            => new ValueTask<object>(new object());

        [Command(CommandName)]
        public object ObjectCommand()
            => new object();

        [Command(CommandName)]
        public void ParameterCommand(string name)
        { }

        [Command(CommandName)]
        internal void InternalMethod()
        { }

        [Command(CommandName)]
        public static void StaticMethod()
        { }

        [Command(CommandName)]
        public void GenericMethod<T>()
        { }

        public void InvalidCommand()
        { }

        public Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommand command, string rawArgs)
            => throw new NotSupportedException();
    }

    public class TestClassPreconditionAttribute : PreconditionAttribute
    {
        public override Task<IPreconditionResult> ExecuteAsync(ICommand command, CommandContext context)
            => throw new NotSupportedException();
    }
}