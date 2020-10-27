using System.Threading.Tasks;

namespace MariCommands.Filters
{
    /// <summary>
    /// Marker interface for filters handled in command execution when has a result.
    /// </summary>
    public interface ICommandResultFilter : ICommandFilter
    {
        Task InvokeAsync(CommandResultContext context, CommandResultDelegate next);
    }
}