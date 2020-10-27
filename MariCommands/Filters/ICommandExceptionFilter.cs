using System.Threading.Tasks;

namespace MariCommands.Filters
{
    public interface ICommandExceptionFilter : ICommandFilter
    {
        Task InvokeAsync(CommandExceptionContext context, CommandExceptionDelegate next);
    }
}