using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MariCommands.Filters
{
    /// <summary>
    /// A filter that can dispose the <see cref="CommandContext" /> when invoked.
    /// </summary>
    public class DisposeFilter : ICommandResultFilter
    {
        /// <inheritdoc />
        public async Task InvokeAsync(CommandResultContext context, CommandResultDelegate next)
        {
            var options = context?.CommandContext.CommandServices?.GetService<IOptions<MariCommandsOptions>>()?.Value;

            if (options.HasNoContent() || !options.AutoDisposeContext)
                return;

            await context.CommandContext.DisposeAsync();

            await next(context);
        }
    }
}