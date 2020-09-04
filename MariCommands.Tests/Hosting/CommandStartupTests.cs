using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Factories;
using MariCommands.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace MariCommands.Tests.Hosting
{
    public class CommandStartupTests
    {
        [Fact]
        public async Task WillCallCommandStartupMethodsWhenInjectInHost()
        {
            var props = new Dictionary<string, object>();

            MariCommandsHostBuilderExtensions.Clear();

            var host = Host.CreateDefaultBuilder()
                            .ConfigureServices(services =>
                            {
                                services.AddTransient<ICommandApplicationBuilderFactory>((sp) =>
                                {
                                    return new TestCommandApplicationBuilderFactory(props);
                                });
                            })
                            .UseCommandStartup<TestCommandStartupAssert>()
                            .Build();

            await host.StartAsync();

            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();

            Assert.NotNull(configurer);

            var logValue = props[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }

        [Fact]
        public async Task WillCallCommandServiceStartupMethodsWhenInjectInHost()
        {
            var props = new Dictionary<string, object>();

            MariCommandsHostBuilderExtensions.Clear();

            var host = Host.CreateDefaultBuilder()
                            .ConfigureServices(services =>
                            {
                                services.AddTransient<ICommandApplicationBuilderFactory>((sp) =>
                                {
                                    return new TestCommandApplicationBuilderFactory(props);
                                });
                            })
                            .UseCommandServiceStartup<TestCommandStartupAssert>()
                            .Build();

            await host.StartAsync();

            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();
            var dependency = host.Services.GetRequiredService<SomeDependencyTest>();

            Assert.NotNull(configurer);
            Assert.NotNull(dependency);

            var logValue = props[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }

        [Fact]
        public async Task WillCallCommandStartupMethodsWhenInjectInWebHost()
        {
            var props = new Dictionary<string, object>();

            var host = Host.CreateDefaultBuilder()
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseTestServer()
                                            .ConfigureServices(services => { })
                                            .Configure(app => { }); ;

                                webBuilder.ConfigureServices(services =>
                                {
                                    services.AddTransient<ICommandApplicationBuilderFactory>((sp) =>
                                    {
                                        return new TestCommandApplicationBuilderFactory(props);
                                    });
                                });

                                MariCommandsHostBuilderExtensions.Clear();
                                webBuilder.UseCommandStartup<TestCommandStartupAssert>();
                            })
                            .Build();

            await host.StartAsync();

            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();

            Assert.NotNull(configurer);

            var logValue = props[TestCommandStartupAssert.LogKey];
            var separator = configurer.Options.Separator;

            Assert.Equal(TestCommandStartupAssert.LogValue, logValue);
            Assert.Equal(TestCommandStartupAssert.Separator, separator);
        }

        [Fact]
        public async Task WillCallCommandServiceStartupMethodsWhenInjectInWebHost()
        {
            var props = new Dictionary<string, object>();

            var host = Host.CreateDefaultBuilder()
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseTestServer()
                                            .ConfigureServices(services => { })
                                            .Configure(app => { });

                                webBuilder.ConfigureServices(services =>
                                {
                                    services.AddTransient<ICommandApplicationBuilderFactory>((sp) =>
                                    {
                                        return new TestCommandApplicationBuilderFactory(props);
                                    });
                                });

                                MariCommandsHostBuilderExtensions.Clear();
                                webBuilder.UseCommandServiceStartup<TestCommandStartupAssert>();
                            })
                            .Build();

            await host.StartAsync();

            var configurer = host.Services.GetRequiredService<IModuleConfigurer>();

            Assert.NotNull(configurer);

            var logValue = props[TestCommandStartupAssert.LogKey];
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
    { }

    public class TestCommandApplicationBuilderFactory : ICommandApplicationBuilderFactory
    {
        private readonly Dictionary<string, object> _props;

        public TestCommandApplicationBuilderFactory(Dictionary<string, object> props)
        {
            _props = props;
        }

        public ICommandApplicationBuilder Create(IDictionary<string, object> properties, IServiceProvider provider)
            => new CommandApplicationBuilder(_props ?? properties, provider);
    }
}