using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Middlewares
{
    internal sealed class CommandContextInitializerMiddleware : ICommandMiddleware
    {
        public Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            var accessor = context.CommandServices.GetService<ICommandContextAccessor>();

            if (accessor.HasContent())
                accessor.CommandContext = context;

            return next(context);
        }
    }
}