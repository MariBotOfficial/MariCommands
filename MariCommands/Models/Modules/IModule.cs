using System;
using System.Collections.Generic;

namespace MariCommands
{
    /// <summary>
    /// Represents a module that can have commands.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// The name for this module.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description for this module.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Any remarks for this module.
        /// </summary>
        string Remarks { get; }

        /// <summary>
        /// The <see cref="RunMode" /> for this module.
        /// </summary>
        RunMode? RunMode { get; }

        /// <summary>
        /// If this module will ignore extra args.
        /// </summary>
        bool? IgnoreExtraArgs { get; }

        /// <summary>
        /// The lifetime for this module.
        /// </summary>
        ModuleLifetime? ModuleLifetime { get; }

        /// <summary>
        /// The default argument parser type for this module.
        /// </summary>
        Type ArgumentParserType { get; }

        /// <summary>
        /// Defines how multi matches will be handled in this module.
        /// </summary>
        MultiMatchHandling? MultiMatchHandling { get; }

        /// <summary>
        /// Aliases for this module.
        /// </summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// Submodules for this module.
        /// </summary>
        IReadOnlyCollection<IModule> Submodules { get; }

        /// <summary>
        /// All atributtes of this module.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// All comands of this module.
        /// </summary>
        IReadOnlyCollection<ICommand> Commands { get; }

        /// <summary>
        /// All preconditions to be passed for this module.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// The parent module of this module.
        /// </summary>
        IModule Parent { get; }

        /// <summary>
        /// The real <see cref="Type" /> of this module.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Indicates if this module is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Enable this module.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable this module.
        /// </summary>
        void Disable();
    }
}