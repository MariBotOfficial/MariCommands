using MariCommands.Builder;
using MariCommands.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Extensions
{
    /// <summary>
    /// Extensions to use in a Command Application Builder.
    /// </summary>
    public static class MariCommandsApplicationBuilderExtensions
    {
        /// <summary>
        /// Add a middleware type to the command request pipeline.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseMiddleware<TMiddleware>(this ICommandApplicationBuilder app)
            where TMiddleware : ICommandMiddleware
        {
            app.Use((next) =>
            {
                return async context =>
                {
                    var middleware = ActivatorUtilities.GetServiceOrCreateInstance(context.ServiceProvider, typeof(TMiddleware)) as ICommandMiddleware;

                    await middleware.InvokeAsync(context, next);
                };
            });

            return app;
        }

        /// <summary>
        /// Use the default Exception middleware that will just log the exception.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseDefaultExceptionMiddleware(this ICommandApplicationBuilder app)
            => app.UseMiddleware<DefaultExceptionMiddleware>();
    }
}