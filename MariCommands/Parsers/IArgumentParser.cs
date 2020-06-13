using System.Threading.Tasks;

namespace MariCommands
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
        /// <returns>A <see cref="Task" /> representing an asynchronous operation with an
        /// <see cref="IArgumentParserResult" />.</returns>
        Task<IArgumentParserResult> ParseAsync(ICommandContext context);
    }
}