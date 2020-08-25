using System;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Hosting;
using MariCommands.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace MariCommands.Tests.Hosting
{
    public class CommandStartupFilterTests
    {
        public CommandStartupFilterTests()
        {
            MariCommandsHostBuilderExtensions.Clear();
        }

        [Fact]
        public async Task WillStartAppInWebHosts()
        {
            var host = Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                            MariCommandsHostBuilderExtensions.Clear();
                            webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                            webBuilder.UseStartup<TestCommandStartup>();
                        })
                        .Build();

            await host.StartAsync();

            var executor = host.Services.GetRequiredService<IContextExecutor>();

            var context = new CommandContext()
            {
                Result = new Mock<IResult>().Object,
                Command = new Mock<ICommand>().Object,
            };

            await executor.ExecuteAsync(string.Empty, context);
        }
    }

    public class TestCommandStartup : ICommandServiceStartup
    {
        public void Configure(IApplicationBuilder app)
        {
        }

        public void ConfigureApp(ICommandApplicationBuilder app)
        {
        }

        public void ConfigureModules(IModuleConfigurer moduleConfigurer)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}