using System;
using System.Linq;
using MariCommands.Builder;
using MariCommands.Middlewares;
using MariCommands.Utils;
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
        /// <param name="middlewareArgs">Additional ctor args for this middleware.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseMiddleware<TMiddleware>(this ICommandApplicationBuilder app, params object[] middlewareArgs)
            where TMiddleware : ICommandMiddleware
        {
            app.Use((next) =>
            {
                var types = middlewareArgs
                                ?.Select(a => a.GetType())
                                ?.ToArray() ?? new Type[0];

                var instanceFactory = ActivatorUtilities.CreateFactory(typeof(TMiddleware), types);

                return async context =>
                {
                    var instance = instanceFactory(context.CommandServices, middlewareArgs);

                    await (instance as ICommandMiddleware).InvokeAsync(context, next);

                    await MiddlewareUtils.SwitchDisposeAsync(instance);
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

        /// <summary>
        /// Use the default command executor middleware.
        /// </summary>
        /// <param name="app">The current command aplication builder.</param>
        /// <returns>The current command aplication builder.</returns>
        public static ICommandApplicationBuilder UseCommandExecutor(this ICommandApplicationBuilder app)
            => app.UseMiddleware<CommandExecutorMiddleware>();
    }
}