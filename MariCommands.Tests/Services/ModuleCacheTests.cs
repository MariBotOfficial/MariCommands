using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace MariCommands.Tests.Services
{
    public class ModuleCacheTests
    {
        [Fact]
        public void CanAddModule()
        {
            var config = new MariCommandsOptions();
            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testCmdAlias",
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testModuleAlias"
            });
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);
        }

        [Fact]
        public void CanRemoveModule()
        {
            var config = new MariCommandsOptions();
            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testCmdAlias",
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testModuleAlias"
            });
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            moduleCache.RemoveModule(module);
        }

        [Fact]
        public void CanGetAllModules()
        {
            var config = new MariCommandsOptions();
            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testCmdAlias",
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testModuleAlias"
            });
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var modules = moduleCache.GetAllModules();

            Assert.NotNull(modules);
            Assert.NotEmpty(modules);
            Assert.Equal(module, modules.FirstOrDefault());
        }

        [Fact]
        public void CanGetAllCommands()
        {
            var config = new MariCommandsOptions();
            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testCmdAlias",
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                "testModuleAlias"
            });
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var allCommands = moduleCache.GetAllCommands();

            Assert.NotNull(allCommands);
            Assert.NotEmpty(allCommands);
            Assert.Equal(command1, allCommands.FirstOrDefault());
        }

        public static IEnumerable<object[]> CommandOneAliasData =>
        new List<object[]>
        {
            new object[] { "Test", "TEST" },
            new object[] { "Test", "test" },
            new object[] { "TEST", "test" },
            new object[] { "TEST", "Test" },
            new object[] { "test", "Test" },
            new object[] { "test", "TEST" },
        };

        [MemberData(nameof(CommandOneAliasData))]
        [Theory]
        public async Task CantFindCommandWithoutIgnoreCaseAndOnceAlias(string alias, string input)
        {
            var config = new MariCommandsOptions();

            config.Comparison = StringComparison.Ordinal;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                alias,
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>());
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.Empty(matches);
        }

        [MemberData(nameof(CommandOneAliasData))]
        [Theory]
        public async Task CanFindCommandWithIgnoreCaseAndOnceAlias(string alias, string input)
        {
            var config = new MariCommandsOptions();
            var comparison = StringComparison.InvariantCultureIgnoreCase;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                alias,
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>());
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.NotEmpty(matches);
            Assert.True(input.Equals(matches.FirstOrDefault().Alias, comparison));
        }

        public static IEnumerable<object[]> CommandInputOneData =>
        new List<object[]>
        {
            new object[] { "TEST", "TEST" },
            new object[] { "test", "test arg1" },
            new object[] { "Test", "Test arg1 arg2" },
            new object[] { "TEST", "TEST arg1 arg2 arg3" },
            new object[] { "test", "test arg1 arg2 arg3 arg4" },
            new object[] { "TEST", "TEST arg1 arg2 arg3 arg4 arg5" },
        };

        [MemberData(nameof(CommandInputOneData))]
        [Theory]
        public async Task CalculateCorrectRemainingInput(string alias, string input)
        {
            var config = new MariCommandsOptions();
            var comparison = StringComparison.Ordinal;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                alias,
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>());
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.NotEmpty(matches);

            var remainingInput = string.Join(config.Separator,
                                    input
                                    .Split(config.Separator)
                                    .Where(a => !a.Equals(alias, comparison))
                                    .ToList());

            var match = matches.FirstOrDefault();

            Assert.True(remainingInput.Equals(match.RemainingInput, comparison));
        }

        public static IEnumerable<object[]> CommandMultipleAliasesData =>
        new List<object[]>
        {
            new object[]
            {
                new string[]
                {
                    "test",
                    "TEST",
                },
                "Test"
            },
            new object[]
            {
                new string[]
                {
                    "Test",
                    "TEST",
                },
                "test"
            },
            new object[]
            {
                new string[]
                {
                    "Test",
                    "test",
                },
                "TEST"
            },
        };

        [MemberData(nameof(CommandMultipleAliasesData))]
        [Theory]
        public async Task CantFindCommandWithoutIgnoreCaseAndMultipleAlias(string[] aliases, string input)
        {
            var config = new MariCommandsOptions();

            var comparison = StringComparison.Ordinal;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(aliases);

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>());
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.Empty(matches);
        }

        [MemberData(nameof(CommandMultipleAliasesData))]
        [Theory]
        public async Task CanFindCommandWithIgnoreCaseAndMultipleAlias(string[] aliases, string input)
        {
            var config = new MariCommandsOptions();

            var comparison = StringComparison.InvariantCultureIgnoreCase;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(aliases);

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>());
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.NotEmpty(matches);
            Assert.True(input.Equals(matches.FirstOrDefault().Alias, comparison));
        }

        public static IEnumerable<object[]> CommandModuleOneAliasData =>
        new List<object[]>
        {
            new object[] { "TestMdl", "TestCmd", "testmdl testcmd" },
            new object[] { "TestMdl", "TestCmd", "testmdl TestCmd" },
            new object[] { "testmdl", "testcmd", "TestMdl TestCmd" },
            new object[] { "testmdl", "testcmd", "TestMdl testcmd" },
            new object[] { "testmdl", "testcmd", "testmdl TestCmd" },
        };

        [MemberData(nameof(CommandModuleOneAliasData))]
        [Theory]
        public async Task CantFindCommandModuleWithoutIgnoreCaseAndOnceAlias(string moduleAlias, string alias, string input)
        {
            var config = new MariCommandsOptions();

            var comparison = StringComparison.Ordinal;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                alias,
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                moduleAlias,
            });
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.Empty(matches);
        }

        [MemberData(nameof(CommandModuleOneAliasData))]
        [Theory]
        public async Task CanFindCommandModuleWithIgnoreCaseAndOnceAlias(string moduleAlias, string alias, string input)
        {
            var config = new MariCommandsOptions();

            var comparison = StringComparison.InvariantCultureIgnoreCase;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                alias,
            });

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(new List<string>
            {
                moduleAlias,
            });
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.NotEmpty(matches);
            Assert.True(input.Equals(matches.FirstOrDefault().Alias, comparison));
        }

        public static IEnumerable<object[]> CommandModuleMultipleAliasesData =>
        new List<object[]>
        {
            new object[]
            {
                new string[]
                {
                    "TestMdl",
                    "testmdl",
                },
                new string[]
                {
                    "TestCmd",
                    "testcmd",
                },
                "TESTMDL TESTCMD"
            },
            new object[]
            {
                new string[]
                {
                    "TestMdl",
                    "TESTMDL",
                },
                new string[]
                {
                    "TestCmd",
                    "TESTCMD",
                },
                "testMdl testcmd"
            },
            new object[]
            {
                new string[]
                {
                    "testmdl",
                    "TESTMDL",
                },
                new string[]
                {
                    "testcmd",
                    "TESTCMD",
                },
                "TestMdl TestCmd"
            },
        };

        [MemberData(nameof(CommandModuleMultipleAliasesData))]
        [Theory]
        public async Task CantFindCommandModuleWithoutIgnoreCaseAndMultipleAlias(string[] moduleAliases, string[] aliases, string input)
        {
            var config = new MariCommandsOptions();

            var comparison = StringComparison.Ordinal;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(aliases);

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(moduleAliases);
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.Empty(matches);
        }

        [MemberData(nameof(CommandModuleMultipleAliasesData))]
        [Theory]
        public async Task CanFindCommandModuleWithIgnoreCaseAndMultipleAlias(string[] moduleAliases, string[] aliases, string input)
        {
            var config = new MariCommandsOptions();

            var comparison = StringComparison.InvariantCultureIgnoreCase;

            config.Comparison = comparison;

            var moduleCache = new ModuleCache(config);

            var commandMock1 = new Mock<ICommand>();

            commandMock1.SetupGet(a => a.Aliases).Returns(aliases);

            var command1 = commandMock1.Object;

            var commands = new List<ICommand>
            {
                command1,
            };

            var moduleMock = new Mock<IModule>();

            moduleMock.SetupGet(a => a.Commands).Returns(commands);
            moduleMock.SetupGet(a => a.Aliases).Returns(moduleAliases);
            moduleMock.SetupGet(a => a.Submodules).Returns(new List<IModule>());

            var module = moduleMock.Object;

            commandMock1.SetupGet(a => a.Module).Returns(module);

            moduleCache.AddModule(module);

            var matches = await moduleCache.SearchCommandsAsync(input);

            Assert.NotNull(matches);
            Assert.NotEmpty(matches);
            Assert.True(input.Equals(matches.FirstOrDefault().Alias, comparison));
        }
    }
}