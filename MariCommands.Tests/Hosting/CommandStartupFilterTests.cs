using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Hosting;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace MariCommands.Tests.Hosting
{
    public class CommandStartupFilterTests
    {
        [Fact]
        public async ValueTask WillStartAppInWebHosts()
        {
            var host = Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            webBuilder.UseCommandServiceStartup<TestCommandStartup>();
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