using System;
using System.Collections.Generic;
using System.Reflection;

namespace MariCommands
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// The name for this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description for this command.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Any remarks for this command.
        /// </summary>
        string Remarks { get; }

        /// <summary>
        /// The priority of this command.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// The <see cref="RunMode" /> for this command.
        /// </summary>
        RunMode? RunMode { get; }

        /// <summary>
        /// If this command will ignore extra args.
        /// </summary>
        bool? IgnoreExtraArgs { get; }

        /// <summary>
        /// The default argument parser type for this command.
        /// </summary>
        Type ArgumentParserType { get; }

        /// <summary>
        /// Aliases for this command.
        /// </summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// All atributtes of this command.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// All preconditions to be passed for this command.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// All parameters of this command.
        /// </summary>
        IReadOnlyCollection<IParameter> Parameters { get; }

        /// <summary>
        /// Indicates if this command is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// The module of this command.
        /// </summary>
        IModule Module { get; }

        /// <summary>
        /// The real <see cref="MethodInfo" /> of this command.
        /// </summary>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Enable this command.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable this command.
        /// </summary>
        void Disable();
    }
}