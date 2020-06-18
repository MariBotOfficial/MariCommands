namespace MariCommands
{
    /// <inheritdoc />
    public class CommandMatch : ICommandMatch
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommandMatch" />.
        /// </summary>
        /// <param name="command">The matched command.</param>
        /// <param name="alias">The alias used to find this command.</param>
        /// <param name="rawArgs">The raw input used.</param>
        /// <param name="remainingInput">The remaining input.</param>
        public CommandMatch(ICommand command, string alias, string rawArgs, string remainingInput)
        {
            Command = command;
            Alias = alias;
            RawArgs = rawArgs;
            RemainingInput = remainingInput;
        }

        /// <inheritdoc />
        public ICommand Command { get; }

        /// <inheritdoc />
        public string Alias { get; }

        /// <inheritdoc />
        public string RawArgs { get; }

        /// <inheritdoc />
        public string RemainingInput { get; }
    }
}