using System;
using Microsoft.Extensions.Options;

namespace MariCommands
{
    /// <summary>
    /// General config for this lib.
    /// </summary>
    public class MariCommandsOptions : IOptions<MariCommandsOptions>
    {
        /// <summary>
        /// Gets or sets the default runmode for commands.
        /// </summary>
        public RunMode RunMode { get; set; } = RunMode.Sequential;

        /// <summary>
        /// Gets or sets the default comparison for search commands.
        /// </summary>
        public StringComparison Comparison { get; set; } = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Gets or sets if extra args in a command will be ignored.
        /// </summary>
        public bool IgnoreExtraArgs { get; set; } = false;

        /// <summary>
        /// Gets or sets how multi matches will be handled.
        /// </summary>
        public MultiMatchHandling MatchHandling { get; set; } = MultiMatchHandling.Error;

        /// <summary>
        /// Gets or sets the default command separator to be used for commands and groups.
        /// </summary>
        public string Separator { get; set; } = " ";

        /// <summary>
        /// Gets or sets if this lib will consider any type parser of class can parse null values.
        /// </summary>
        public bool TypeParserOfClassIsNullables { get; set; } = false;

        /// <summary>
        /// Gets or sets if this lib will auto add the running assembly in modules.
        /// </summary>
        public bool AutoAddRunningAssembly { get; set; } = false;

        /// <summary>
        /// Gets or sets if this lib will continue to handle multi matches after parsing.
        /// </summary>
        public bool ContinueMultiMatchAfterParser { get; set; } = false;

        /// <summary>
        /// Gets or sets if this lib will auto dispose the <see cref="CommandContext" /> after 
        /// execute a command, the default is <c>true</c>.
        /// </summary>
        public bool AutoDisposeContext { get; set; } = true;

        /// <summary>
        /// Gets or sets If this lib wil inject all default type parsers to the dependency,
        /// the default is <c>true</c>.
        /// </summary>
        public bool AddAllDefaultTypeParsers { get; set; } = true;

        /// <summary>
        /// Gets or sets if this lib will create nullables type parsers for these type parsers,
        /// the default is <c>true</c>.
        /// </summary>
        public bool CreateNullables { get; set; } = true;

        MariCommandsOptions IOptions<MariCommandsOptions>.Value
            => this;
    }
}