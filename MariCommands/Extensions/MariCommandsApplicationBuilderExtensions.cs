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
                    var middleware = ActivatorUtilities.GetServiceOrCreateInstance(context.CommandServices, typeof(TMiddleware)) as ICommandMiddleware;

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

        /// <summary>
        /// Use the default command string matcher middleware.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseStringMatcher(this ICommandApplicationBuilder app)
            => app.UseMiddleware<CommandStringMatcherMiddleware>();

        /// <summary>
        /// Use the default command input count matcher middleware.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseInputCountMatcher(this ICommandApplicationBuilder app)
            => app.UseMiddleware<CommandInputCountMatcherMiddleware>();

        /// <summary>
        /// Use the default command parser middleware.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseParser(this ICommandApplicationBuilder app)
            => app.UseMiddleware<CommandParserMiddleware>();

        /// <summary>
        /// Use the default command param preconditioner middleware.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseParamPreconditioner(this ICommandApplicationBuilder app)
            => app.UseMiddleware<CommandParamPreconditionMiddleware>();

        /// <summary>
        /// Use the default command preconditioner middleware.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UsePreconditioner(this ICommandApplicationBuilder app)
            => app.UseMiddleware<CommandPreconditionMiddleware>();
    }
}