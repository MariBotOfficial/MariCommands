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
        public StringComparison Comparison { get; set; } = StringComparison.OrdinalIgnoreCase;

        /// <inheritdoc />
        public bool IgnoreExtraArgs { get; set; } = false;

        /// <inheritdoc />
        public MultiMatchHandling MatchHandling { get; set; } = MultiMatchHandling.Error;

        /// <inheritdoc />
        public string Separator { get; set; } = " ";

        /// <inheritdoc />
        public bool TypeParserOfClassIsNullables { get; set; } = false;

        /// <inheritdoc />
        public bool AutoAddRunningAssembly { get; set; } = false;

        /// <inheritdoc />
        public bool ContinueMultiMatchAfterParser { get; set; } = false;
    }
}