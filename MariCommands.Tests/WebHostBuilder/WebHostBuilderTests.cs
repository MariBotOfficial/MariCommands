using System;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Tests.HostBuilder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace MariCommands.Tests.WebHostBuilder
{
    public class WebHostBuilderTests
    {
        public WebHostBuilderTests()
        {
            MariCommandsHostBuilderExtensions.Clear();
        }

        [Fact]
        public void TwoStartupsThrowsException()
        {
            MariCommandsHostBuilderExtensions.Clear();

            var builder = Host.CreateDefaultBuilder();

            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                builder
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .Configure(app => { })
                                        .ConfigureServices(services => { });

                        webBuilder.UseCommandStartup<TestCommandStartup>();
                        webBuilder.UseCommandStartup<TestCommandStartup>();
                    })
                    .Build();
            });
        }

        [Fact]
        public void OneStartupNotThrowsException()
        {
            Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                        // A lol bug with statics in xUnit.
                        MariCommandsHostBuilderExtensions.Clear();
                        webBuilder.UseCommandStartup<TestCommandStartup>();
                    })
                    .Build();
        }

        [Fact]
        public async Task CanExecuteAnyWorkerCommandAfterAddCommandStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                        MariCommandsHostBuilderExtensions.Clear();

                        webBuilder.UseCommandStartup<TestCommandStartup>();
                        webBuilder.ConfigureServices(services =>
                        {
                            services.AddHostedService<TestWorkerService>();
                        });
                    })
                    .Build();

            await host.StartAsync();
        }

        [Fact]
        public async Task CantExecuteAnyWorkerCommandBeforeAddCommandStartup()
        {
            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                var host = Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                            webBuilder.ConfigureServices(services =>
                            {
                                services.AddHostedService<TestWorkerService>();
                            });
                            webBuilder.UseCommandStartup<TestCommandStartup>();
                        })
                        .Build();

                await host.StartAsync();
            });
        }
        [Fact]
        public async Task CanExecuteAnyCommandAfterAddCommandStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                        webBuilder.UseCommandStartup<TestCommandStartup>();
                    })
                    .Build();

            await host.StartAsync();

            var contextExecutor = host.Services.GetRequiredService<IContextExecutor>();

            await contextExecutor.ExecuteAsync(string.Empty, new CommandContext());
        }


        [Fact]
        public void TwoCommandServiceStartupsThrowsException()
        {
            var builder = Host.CreateDefaultBuilder();

            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                builder.ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                    webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                    webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                })
                .Build();
            });
        }

        [Fact]
        public void OneCommandServiceStartupNotThrowsException()
        {
            Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                        MariCommandsHostBuilderExtensions.Clear();
                        webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                    })
                    .Build();
        }

        [Fact]
        public async Task CanExecuteAnyCommandAfterAddCommandServiceStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                        MariCommandsHostBuilderExtensions.Clear();
                        webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                    })
                    .Build();

            await host.StartAsync();

            var contextExecutor = host.Services.GetRequiredService<IContextExecutor>();

            await contextExecutor.ExecuteAsync(string.Empty, new CommandContext());
        }

        [Fact]
        public async Task CanExecuteAnyWorkerCommandAfterAddCommandServiceStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                        webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                        webBuilder.ConfigureServices(services =>
                        {
                            services.AddHostedService<TestWorkerService>();
                        });
                    })
                    .Build();

            await host.StartAsync();
        }

        [Fact]
        public async Task CantExecuteAnyWorkerCommandBeforeAddCommandServiceStartup()
        {
            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                var host = Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            webBuilder.UseTestServer()
                                        .ConfigureServices(services => { })
                                        .Configure(app => { });

                            webBuilder.ConfigureServices(services =>
                            {
                                services.AddHostedService<TestWorkerService>();
                            });
                            webBuilder.UseCommandServiceStartup<TestCommandStartup>();
                        })
                        .Build();

                await host.StartAsync();
            });
        }
    }
}