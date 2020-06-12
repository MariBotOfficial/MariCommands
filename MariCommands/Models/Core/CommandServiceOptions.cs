using System;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandServiceOptions : ICommandServiceOptions
    {
        /// <inheritdoc />
        public ModuleLifetime ModuleLifetime { get; set; } = ModuleLifetime.Transient;

        /// <inheritdoc />
        public RunMode RunMode { get; set; } = RunMode.Sequential;

        /// <inheritdoc />
        public StringComparison? Comparison { get; set; } = null;

        /// <inheritdoc />
        public bool IgnoreExtraArgs { get; set; } = false;

        /// <inheritdoc />
        public MultiMatchHandling MatchHandling { get; set; } = MultiMatchHandling.Error;

        /// <inheritdoc />
        public string Separator { get; set; } = " ";
    }
}