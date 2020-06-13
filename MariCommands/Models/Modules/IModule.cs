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
        /// The <see cref="RunMode" /> of this module.
        /// </summary>
        RunMode RunMode { get; }

        /// <summary>
        /// If this module will ignore extra args.
        /// </summary>
        bool IgnoreExtraArgs { get; }

        /// <summary>
        /// The lifetime of this module.
        /// </summary>
        ModuleLifetime ModuleLifetime { get; }

        /// <summary>
        /// Aliases for this module.
        /// </summary>
        IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// Submoduels for this module.
        /// </summary>
        IReadOnlyCollection<IModule> Submodules { get; }

        /// <summary>
        /// All atributtes of this module.
        /// </summary>
        IReadOnlyCollection<Attribute> Attributes { get; }

        // TODO: IReadOnlyCollection<ICommand> Commands { get; }

        // TODO: IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        /// The parent module of this module.
        /// </summary>
        IModule Parent { get; }

        /// <summary>
        /// The real <see cref="Type" /> of this module.
        /// </summary>
        Type Type { get; }
    }
}