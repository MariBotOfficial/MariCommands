using System.Threading.Tasks;

namespace MariCommands.Filters
{
    public delegate Task CommandExceptionDelegate(CommandExceptionContext context);
}