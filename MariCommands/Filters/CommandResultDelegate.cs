using System.Threading.Tasks;

namespace MariCommands.Filters
{
    public delegate Task CommandResultDelegate(CommandResultContext context);
}