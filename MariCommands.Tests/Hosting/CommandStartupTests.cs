using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace MariCommands.Tests.Hosting
{
    public class CommandStartupTests
    {
        [Fact]
        public async ValueTask WillCallCommandStartupMethodsWhenInjectInHost()
        {
            var host = Host.CreateDefaultBuilder()
                            .UseCommandStartup<TestCommandStartupAssert>()
                            .Build();

            await host.StartAsync();

            var app = host.Services.GetRequiredService<ICommandApplicationBuilder>();
            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();

            Assert.NotNull(app);
            Assert.NotNull(configurer);

            var logValue = app.Properties[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }

        [Fact]
        public async ValueTask WillCallCommandServiceStartupMethodsWhenInjectInHost()
        {
            var host = Host.CreateDefaultBuilder()
                            .UseCommandServiceStartup<TestCommandStartupAssert>()
                            .Build();

            await host.StartAsync();

            var app = host.Services.GetRequiredService<ICommandApplicationBuilder>();
            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();
            var dependency = host.Services.GetRequiredService<SomeDependencyTest>();

            Assert.NotNull(app);
            Assert.NotNull(configurer);
            Assert.NotNull(dependency);

            var logValue = app.Properties[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }

        [Fact]
        public async ValueTask WillCallCommandStartupMethodsWhenInjectInWebHost()
        {
            var host = Host.CreateDefaultBuilder()
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseCommandStartup<TestCommandStartupAssert>();
                            })
                            .Build();

            await host.StartAsync();

            var app = host.Services.GetRequiredService<ICommandApplicationBuilder>();
            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();
            var dependency = host.Services.GetRequiredService<SomeDependencyTest>();

            Assert.NotNull(app);
            Assert.NotNull(configurer);
            Assert.NotNull(dependency);

            var logValue = app.Properties[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }

        [Fact]
        public async ValueTask WillCallCommandServiceStartupMethodsWhenInjectInWebHost()
        {
            var host = Host.CreateDefaultBuilder()
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseCommandServiceStartup<TestCommandStartupAssert>();
                            })
                            .Build();

            await host.StartAsync();

            var app = host.Services.GetRequiredService<ICommandApplicationBuilder>();
            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();

            Assert.NotNull(app);
            Assert.NotNull(configurer);

            var logValue = app.Properties[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }
    }

    public class TestCommandStartupAssert : ICommandServiceStartup
    {
        public const string LogKey = "Key";
        public const string LogValue = "Value";
        public const string Separator = "Separator";

        public void ConfigureApp(ICommandApplicationBuilder app)
        {
            Assert.NotNull(app);

            app.Properties[LogKey] = LogValue;
        }

        public void ConfigureModules(IModuleConfigurer moduleConfigurer)
        {
            Assert.NotNull(moduleConfigurer);

            moduleConfigurer.Options.Separator = Separator;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Assert.NotNull(services);

            services.AddTransient<SomeDependencyTest>();
        }
    }

    public class SomeDependencyTest
    {

    }
}