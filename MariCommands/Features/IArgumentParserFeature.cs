using System.Collections.Generic;

namespace MariCommands.Features
{
    /// <summary>
    /// A feature that can gets or sets parser infos about a command match.
    /// </summary>
    public interface IArgumentParserFeature
    {
        /// <summary>
        /// A collection of key value pair where the key is a command match and the value the parsed args.
        /// </summary>
        IDictionary<ICommandMatch, IReadOnlyDictionary<IParameter, object>> CommandArgs { get; set; }
    }
}