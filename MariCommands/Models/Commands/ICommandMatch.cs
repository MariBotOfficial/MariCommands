namespace MariCommands
{
    /// <summary>
    /// Represents a matched command in search.
    /// </summary>
    public interface ICommandMatch
    {
        /// <summary>
        /// The matched command.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// The alias used to find this command.
        /// </summary>
        string Alias { get; }

        /// <summary>
        /// The raw input used to search this command.
        /// </summary>
        string RawArgs { get; }

        /// <summary>
        /// The remaining input.
        /// </summary>
        string RemainingInput { get; }
    }
}