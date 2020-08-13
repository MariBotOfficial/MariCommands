using System.Threading.Tasks;
using MariCommands.Results;

namespace MariCommands.Parsers
{
    /// <summary>
    /// A parser for parse params to the specified types.
    /// </summary>
    public interface IArgumentParser
    {
        /// <summary>
        /// Asynchronously parse all raw args to the specified type.
        /// </summary>
        /// <param name="context">The current command execution context.</param>
        /// <param name="command">The command to be parsed.</param>
        /// <param name="rawArgs">The raw args to be used to parse the parameters.</param>
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="IArgumentParserResult" />.</returns>
        Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommand command, string rawArgs);
    }
}