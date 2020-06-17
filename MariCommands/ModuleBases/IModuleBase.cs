using System.Threading.Tasks;

namespace MariCommands
{
    internal interface IModuleBase
    {
        Task OnCommandExecutedAsync();

        Task OnCommandExecutingAsync();

        internal void SetContext(CommandContext context);
    }
}