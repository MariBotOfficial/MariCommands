using System.Reflection;

namespace MariCommands
{
    /// <summary>
    /// A service that can build and create commands.
    /// </summary>
    public interface IParameterFactory
    {
        /// <summary>
        /// Build a parameter for the specified <see cref="ParameterInfo" />.
        /// </summary>
        /// <param name="command">The command builder of this parameter.</param>
        /// <param name="parameterInfo">The real parameter info.</param>
        /// <returns>A builded parameter.</returns>
        IParameterBuilder BuildParameter(ICommandBuilder command, ParameterInfo parameterInfo);

        /// <summary>
        /// Verify if the specified <see cref="ParameterInfo" /> is a parameter.
        /// </summary>
        /// <param name="command">The command builder of this parameter.</param>
        /// <param name="parameterInfo">The real parameter info to be verified.</param>
        /// <returns>If this is a valid parameter.</returns>
        bool IsParameter(ICommandBuilder command, ParameterInfo parameterInfo);
    }
}