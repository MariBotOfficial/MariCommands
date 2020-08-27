using MariCommands.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace MariCommands.Tests.Providers
{
    public class HostBuilderServiceProviderTests
    {
        [Fact]
        public void CanGetConfiguration()
        {
            var hostEnvironment = new Mock<IHostEnvironment>().Object;
            var configuration = new Mock<IConfiguration>().Object;
            var provider = new HostBuilderServiceProvider(configuration, hostEnvironment);

            var result = provider.GetService(typeof(IConfiguration));

            Assert.NotNull(result);
        }

        [Fact]
        public void CanGetHostEnvironment()
        {
            var hostEnvironment = new Mock<IHostEnvironment>().Object;
            var configuration = new Mock<IConfiguration>().Object;
            var provider = new HostBuilderServiceProvider(configuration, hostEnvironment);

            var result = provider.GetService(typeof(IHostEnvironment));

            Assert.NotNull(result);
        }

        [Fact]
        public void NullForAnyOtherServiceType()
        {
            var hostEnvironment = new Mock<IHostEnvironment>().Object;
            var configuration = new Mock<IConfiguration>().Object;
            var provider = new HostBuilderServiceProvider(configuration, hostEnvironment);

            var result = provider.GetService(typeof(HostBuilderServiceProviderTests));

            Assert.Null(result);
        }
    }
}