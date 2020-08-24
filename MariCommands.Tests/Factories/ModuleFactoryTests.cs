using System;
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
    public class ModuleFactoryTests
    {
        private readonly ModuleFactory _factory;
        private readonly CommandFactory _commandFactory;

        public ModuleFactoryTests()
        {
            var options = new CommandServiceOptions();
            var parameterFactory = new ParameterFactory(options);
            var executorProvider = new CommandExecutorProvider();
            var commandFactory = new CommandFactory(options, parameterFactory, executorProvider);
            var factory = new ModuleFactory(options, commandFactory);

            _factory = factory;
            _commandFactory = commandFactory;
        }

        [Fact]
        public void IsValidModule()
        {
            var module = typeof(TestModuleClass);

            var result = _factory.IsModule(module);

            Assert.True(result);
        }

        [Fact]
        public void IsValidSubModule()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);

            var result = _factory.IsSubModule(module);

            Assert.True(result);
        }

        [Fact]
        public void InvalidIfModuleTypeIsNull()
        {
            var result = _factory.IsModule(null);

            Assert.False(result);
        }

        [Fact]
        public void InvalidIfSubModuleTypeIsNull()
        {
            var result = _factory.IsSubModule(null);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfNotClass()
        {
            var module = typeof(ModuleNotClass);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfNotClass()
        {
            var module = typeof(ModuleNotClass.SubModuleNotClass);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfAbstract()
        {
            var module = typeof(AbstractModule);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfAbstract()
        {
            var module = typeof(AbstractModule.AbstractSubModule);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfNested()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfNotNested()
        {
            var module = typeof(TestModuleClass);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfIsNotPublic()
        {
            var module = typeof(InternalModule);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfIsNotPublic()
        {
            var module = typeof(InternalModule.InternalSubModule);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfGeneric()
        {
            var module = typeof(GenericModule<>);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfGeneric()
        {
            var module = typeof(GenericModule<>.GenericSubModule);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfNotModuleBase()
        {
            var module = typeof(NotModuleBaseModule);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfNotModuleBase()
        {
            var module = typeof(NotModuleBaseModule.NotModuleBaseSubModule);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidModuleIfDontLoad()
        {
            var module = typeof(DontLoadModule);

            var result = _factory.IsModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfDontLoad()
        {
            var module = typeof(DontLoadModule.DontLoadBaseModule);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void InvalidSubModuleIfDontHasGroup()
        {
            var module = typeof(TestModuleClass.DontGroupSubModule);

            var result = _factory.IsSubModule(module);

            Assert.False(result);
        }

        [Fact]
        public void ThrowsExceptionIfInvalidModule()
        {
            var module = typeof(ModuleNotClass);

            Assert.Throws<ArgumentException>(() =>
            {
                _ = _factory.BuildModule(null, module);
            });
        }

        [Fact]
        public void ThrowsExceptionIfInvalidSubModule()
        {
            var module = typeof(ModuleNotClass.SubModuleNotClass);

            Assert.Throws<ArgumentException>(() =>
            {
                _ = _factory.BuildModule(null, module);
            });
        }

        [Fact]
        public void CanGetNameFromAttribute()
        {
            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Name));
            Assert.Equal(TestModuleClass.ModuleName, builder.Name);
        }

        [Fact]
        public void CanGetNameFromType()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);
            var parentModuleBuilder = new Mock<IModuleBuilder>().Object;

            var builder = _factory.BuildModule(parentModuleBuilder, module);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Name));
            Assert.Equal(typeof(TestModuleClass.TestSubModuleClass).Name, builder.Name);
        }

        [Fact]
        public void CanGetDescriptionIfExists()
        {
            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Description));
            Assert.Equal(TestModuleClass.ModuleDescription, builder.Description);
        }

        [Fact]
        public void CantGetDescriptionIfNotExists()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);
            var parentModuleBuilder = new Mock<IModuleBuilder>().Object;

            var builder = _factory.BuildModule(parentModuleBuilder, module);

            Assert.NotNull(builder);
            Assert.True(string.IsNullOrWhiteSpace(builder.Description));
        }

        [Fact]
        public void CanGetRemarksIfExists()
        {
            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.False(string.IsNullOrWhiteSpace(builder.Remarks));
            Assert.Equal(TestModuleClass.ModuleRemarks, builder.Remarks);
        }

        [Fact]
        public void CantGetRemarksIfNotExists()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);
            var parentModuleBuilder = new Mock<IModuleBuilder>().Object;

            var builder = _factory.BuildModule(parentModuleBuilder, module);

            Assert.NotNull(builder);
            Assert.True(string.IsNullOrWhiteSpace(builder.Remarks));
        }

        [Fact]
        public void CanGetCustomRunMode()
        {
            // RunMode is a enum and if we can't retrieve it from the module
            // it will be the configured value, that have default value as "Sequential".

            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotNull(builder.RunMode);
            Assert.Equal(TestModuleClass.ModuleRunMode, builder.RunMode);
        }

        [Fact]
        public void CanGetCustomIgnoreExtraArgs()
        {
            // IgnoreExtraArgs is a boolean and if we can't retrieve it from the module
            // it will be the configuration value, that have default value as "false"

            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.Equal(TestModuleClass.ModuleIgnoreExtraArgs, builder.IgnoreExtraArgs);
        }

        [Fact]
        public void CanGetArgumentParserTypeIfExists()
        {
            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotNull(builder.ArgumentParserType);
            Assert.Equal(typeof(TestModuleClass), builder.ArgumentParserType);
        }

        [Fact]
        public void CantGetArgumentParserTypeIfNotExists()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);
            var parentModuleBuilder = new Mock<IModuleBuilder>().Object;

            var builder = _factory.BuildModule(parentModuleBuilder, module);

            Assert.NotNull(builder);
            Assert.Null(builder.ArgumentParserType);
        }

        [Fact]
        public void CanGetCustomMultiMatchHandling()
        {
            // MultiMatchHandling is a enum and if we can't retrieve it from the module
            // it will be the configuration value, that have default value as "Error"

            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotNull(builder.MultiMatchHandling);
            Assert.Equal(TestModuleClass.ModuleMultiMatchHandling, builder.MultiMatchHandling);
        }

        [Fact]
        public void CanGetAliasesIfExists()
        {
            var module = typeof(TestModuleClass.TestSubModuleClass);
            var parentModuleBuilder = new Mock<IModuleBuilder>().Object;

            var builder = _factory.BuildModule(parentModuleBuilder, module);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Aliases);
            Assert.Single(builder.Aliases);
        }

        [Fact]
        public void CantGetAliasesIfNotExists()
        {
            var module = typeof(TestModuleClass);

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.Null(builder.Aliases);
            Assert.Equal(0, builder.Aliases.Count);
        }

        [Fact]
        public void CanGetDisabled()
        {
            var module = typeof(TestModuleClass.DisabledSubModule);
            var parentModuleBuilder = new Mock<IModuleBuilder>().Object;

            var builder = _factory.BuildModule(parentModuleBuilder, module);

            Assert.NotNull(builder);
            Assert.False(builder.IsEnabled);
        }

        [Fact]
        public void CanGetAllAttributes()
        {
            var module = typeof(TestModuleClass);

            var attributes = module.GetCustomAttributes();

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Attributes);
            Assert.Equal(attributes.Count(), builder.Attributes.Count);
        }

        [Fact]
        public void CanGetAllPreconditions()
        {
            var module = typeof(TestModuleClass);

            var preconditions = module.GetCustomAttributes()
                                    .Where(a => typeof(PreconditionAttribute).IsAssignableFrom(a.GetType()))
                                    .ToArray();

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Preconditions);
            Assert.Equal(preconditions.Count(), builder.Preconditions.Count);
        }

        [Fact]
        public void CanGetSubModules()
        {
            var module = typeof(TestModuleClass);

            var submodules = module.GetNestedTypes()
                                    .Where(a => _factory.IsSubModule(a))
                                    .ToArray();

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotEmpty(builder.Submodules);
            Assert.Equal(submodules.Count(), builder.Submodules.Count);
        }

        [Fact]
        public void CanGetCommands()
        {
            var module = typeof(TestModuleClass);
            var fakeBuilder = new Mock<IModuleBuilder>().Object;

            var commands = module.GetMethods()
                                .Where(a => _commandFactory.IsCommand(fakeBuilder, a))
                                .ToArray();

            var builder = _factory.BuildModule(null, module);

            Assert.NotNull(builder);
            Assert.NotNull(builder.Commands);
            Assert.Equal(commands.Length, builder.Commands.Count);
        }
    }

    [Name(ModuleName)]
    [Description(ModuleDescription)]
    [Remarks(ModuleRemarks)]
    [RunMode(ModuleRunMode)]
    [IgnoreExtraArgs(ModuleIgnoreExtraArgs)]
    [ArgumentParser(typeof(TestModuleClass))]
    [MultiMatchHandling(ModuleMultiMatchHandling)]
    [TestClassPrecondition]
    public class TestModuleClass : ModuleBase<CommandContext>, IArgumentParser
    {
        public const string ModuleName = "testName";
        public const string ModuleDescription = "test description";
        public const string ModuleRemarks = "test remarks";
        public const RunMode ModuleRunMode = RunMode.Concurrent;
        public const bool ModuleIgnoreExtraArgs = true;
        public const MultiMatchHandling ModuleMultiMatchHandling = MultiMatchHandling.Best;

        public const string SubModuleAlias = "test";

        public const string CommandName = "testName";

        [Command(CommandName)]
        public void TestCommand()
        { }

        public Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommand command, string rawArgs)
            => throw new NotSupportedException();

        [Group(SubModuleAlias)]
        public class TestSubModuleClass : ModuleBase<CommandContext>
        { }

        public class DontGroupSubModule : ModuleBase<CommandContext>
        { }

        [Disabled]
        [Group(SubModuleAlias)]
        public class DisabledSubModule : ModuleBase<CommandContext>
        { }
    }

    public struct ModuleNotClass : IModuleBase
    {
        public Task OnCommandExecutedAsync()
            => throw new NotSupportedException();

        public Task OnCommandExecutingAsync()
            => throw new NotSupportedException();

        void IModuleBase.SetContext(CommandContext context)
            => throw new NotSupportedException();

        public struct SubModuleNotClass : IModuleBase
        {
            public Task OnCommandExecutedAsync()
                => throw new NotSupportedException();

            public Task OnCommandExecutingAsync()
                => throw new NotSupportedException();

            void IModuleBase.SetContext(CommandContext context)
                => throw new NotSupportedException();
        }
    }

    public abstract class AbstractModule : ModuleBase<CommandContext>
    {
        [Group(TestModuleClass.SubModuleAlias)]
        public abstract class AbstractSubModule : ModuleBase<CommandContext>
        { }
    }

    internal class InternalModule : ModuleBase<CommandContext>
    {
        [Group(TestModuleClass.SubModuleAlias)]
        internal class InternalSubModule : ModuleBase<CommandContext>
        { }
    }

    public class GenericModule<T> : ModuleBase<CommandContext>
    {
        [Group(TestModuleClass.SubModuleAlias)]
        public class GenericSubModule : ModuleBase<CommandContext>
        { }
    }

    public class NotModuleBaseModule
    {
        [Group(TestModuleClass.SubModuleAlias)]
        public class NotModuleBaseSubModule
        { }
    }

    [DontLoad]
    public class DontLoadModule : ModuleBase<CommandContext>
    {
        [DontLoad]
        [Group(TestModuleClass.SubModuleAlias)]
        public class DontLoadBaseModule : ModuleBase<CommandContext>
        { }
    }
}