using System.Threading.Tasks;

namespace MariCommands
{
    public delegate Task CommandDelegate(CommandContext context);
}