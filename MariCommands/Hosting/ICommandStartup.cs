using MariCommands.Builder;

namespace MariCommands.Hosting
{
    /// <summary>
    /// Represents a startup to configure your command app.
    /// </summary>
    public interface ICommandStartup
    {
        /// <summary>
        /// Configure the current middleware pipeline command execution.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        void ConfigureApp(ICommandApplicationBuilder app);
    }
}