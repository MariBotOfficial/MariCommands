using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Hosting
{
    /// <summary>
    /// Represents a startup to configure your command app with your services.
    /// </summary>
    public interface ICommandServiceStartup : ICommandStartup
    {
        /// <summary>
        /// Configure the services for the current command app.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        void ConfigureServices(IServiceCollection services);
    }
}