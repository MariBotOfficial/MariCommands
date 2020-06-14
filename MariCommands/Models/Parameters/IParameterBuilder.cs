using System;
using System.Collections.Generic;
using System.Reflection;

namespace MariCommands
{
    /// <summary>
    /// Represents a pre builded parameter.
    /// </summary>
    public interface IParameterBuilder
    {
        /// <summary>
        /// The name of this parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of this parameter.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Any remarks of this parameter.
        /// </summary>
        string Remarks { get; }

        /// <summary>
        /// Indicates if this parameter is remainder.
        /// </summary>
        bool IsRemainder { get; }

        /// <summary>
        /// Indicates if this parameter use the <see langword="params" /> langword.
        /// </summary>
        bool IsParams { get; }

        /// <summary>
        /// Indicates if this parameter is optional.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// The default value of this parameter.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// The type parser of this parameter.
        /// </summary>
        Type TypeParserType { get; }

        /// <summary>
        /// All preconditions for this parameter.
        /// </summary>
        IReadOnlyCollection<ParamPreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// All attributes of this parameter.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// The real <see cref="Type" /> of this parameter.
        /// </summary>
        ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// The command of this parameter.
        /// </summary>
        ICommandBuilder Command { get; }
    }
}