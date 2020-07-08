using MariCommands.Builder;

namespace MariCommands.Hosting
{
    /// <summary>
    /// Represents a startup to configure your command app.
    /// </summary>
    public interface ICommandStartup
    {
        /// <summary>
        /// Configure the modules for the current command app.
        /// </summary>
        /// <param name="moduleConfigurer">The module configurer to be configured.</param>
        void ConfigureModules(IModuleConfigurer moduleConfigurer);

        /// <summary>
        /// Configure the current middleware pipeline command execution.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        void ConfigureApp(ICommandApplicationBuilder app);
    }
}