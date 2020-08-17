namespace MariCommands
{
    // This implementation is just a scoped dependency, doesn't anything too complex.
    internal sealed class ScopedCommandContextAccessor : ICommandContextAccessor
    {
        public CommandContext CommandContext { get; set; }
    }
}