using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Samples
{
    public class SampleTestServices
    {
        [Fact]
        public async Task CanRunAsync()
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddBasicMariCommandsServices();

            var provider = services.BuildServiceProvider(true);

            provider.InitializeCommandApp();

            var moduleConfigurer = provider.GetRequiredService<IModuleConfigurer>();

            moduleConfigurer.AddModule<SampleTestModuleService>();

            var contextExecutor = provider.GetRequiredService<IContextExecutor>();

            var context = new CommandContext();

            var result = await contextExecutor.ExecuteAsync(SampleTestModuleService.CommandName, context);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(SampleTestModuleService.LogValue, (result as SuccessObjectResult).Value);
        }
    }

    public class SampleTestModuleService : ModuleBase<CommandContext>
    {
        public const string CommandName = "testName";
        public const string LogValue = "Value";

        [Command(CommandName)]
        public Task<IResult> ExecuteAsync()
            => Task.FromResult<IResult>(new SuccessObjectResult(LogValue));
    }
}